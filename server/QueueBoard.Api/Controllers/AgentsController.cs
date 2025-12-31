using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using QueueBoard.Api.DTOs;

namespace QueueBoard.Api.Controllers
{
    /// <summary>
    /// Controller for managing agents: list and fetch by id.
    /// </summary>
    [ApiController]
    [Route("agents")]
    public class AgentsController : ControllerBase 
    {
        private readonly QueueBoardDbContext _db;
        private readonly QueueBoard.Api.Services.IAgentService _svc;
        private readonly Microsoft.Extensions.Logging.ILogger<AgentsController> _logger;

        public AgentsController(QueueBoardDbContext db, QueueBoard.Api.Services.IAgentService svc, Microsoft.Extensions.Logging.ILogger<AgentsController> logger)
        {
            _db = db;
            _svc = svc;
            _logger = logger;
        }

        /// <summary>
        /// Returns a paginated list of agents. Supports optional search, paging and page size.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 25)
        {
            _logger?.LogDebug("GetAll agents {Search} page {Page} size {PageSize}", search, page, pageSize);
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 25;

            var query = _db.Agents.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                query = query.Where(a => a.FirstName.Contains(s) || a.LastName.Contains(s) || a.Email.Contains(s));
            }

            var total = await query.CountAsync();

            var itemsRaw = await query
                .OrderBy(a => a.LastName).ThenBy(a => a.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new { a.Id, a.FirstName, a.LastName, a.Email, a.IsActive, a.CreatedAt, a.UpdatedAt })
                .ToListAsync();

            var items = itemsRaw
                .Select(a => new AgentDto(a.Id, a.FirstName, a.LastName, a.Email, a.IsActive, a.CreatedAt, System.Convert.ToBase64String(System.BitConverter.GetBytes(a.UpdatedAt.UtcTicks))))
                .ToList();

            _logger?.LogInformation("Returned {Count} agents (total {Total}) for page {Page}", items.Count, total, page);
            return Ok(new { totalCount = total, page, pageSize, items });
        }

        /// <summary>
        /// Returns a single agent by id.
        /// </summary>
        /// <param name="id">The agent id (GUID).</param>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] System.Guid id)
        {
            _logger?.LogDebug("GetById agent {Id}", id);
            var dto = await _svc.GetByIdAsync(id);
            if (dto is null)
            {
                _logger?.LogInformation("Agent {Id} not found", id);
                return NotFound();
            }
            Response.Headers["ETag"] = $"\"{dto.RowVersion}\"";
            _logger?.LogInformation("Returned agent {Id}", id);
            return Ok(dto);
        }

        /// <summary>
        /// Update an existing agent. Requires either `RowVersion` in the body or an `If-Match` header with the ETag provided by a previous GET/POST.
        /// On stale token, throws `DbUpdateConcurrencyException` which is mapped to 409 Conflict.
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update([FromRoute] System.Guid id, [FromBody] DTOs.UpdateAgentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var entity = await _db.Agents.FindAsync(id);
            if (entity is null) return NotFound();

            // Accept RowVersion from body or If-Match header (ETag)
            string? headerIfMatch = null;
            if (Request.Headers.TryGetValue("If-Match", out var ifMatchVals))
            {
                headerIfMatch = ifMatchVals.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(headerIfMatch))
                {
                    headerIfMatch = headerIfMatch.Trim();
                    if (headerIfMatch.StartsWith("W/")) headerIfMatch = headerIfMatch.Substring(2).Trim();
                    if (headerIfMatch.StartsWith("\"") && headerIfMatch.EndsWith("\"")) headerIfMatch = headerIfMatch[1..^1];
                }
            }

            var tokenSource = headerIfMatch ?? dto.RowVersion;
            if (string.IsNullOrWhiteSpace(tokenSource))
            {
                ModelState.AddModelError("RowVersion", "RowVersion (body) or If-Match header is required for update.");
                return ValidationProblem(ModelState);
            }

            byte[] tokenBytes;
            try
            {
                tokenBytes = System.Convert.FromBase64String(tokenSource!);
            }
            catch
            {
                ModelState.AddModelError("RowVersion", "RowVersion must be a valid base64 string.");
                return ValidationProblem(ModelState);
            }

            if (tokenBytes.Length < 8)
            {
                ModelState.AddModelError("RowVersion", "RowVersion token is invalid.");
                return ValidationProblem(ModelState);
            }

            // Delegate update to service which will perform concurrency validation and persist changes
            try
            {
                await _svc.UpdateAsync(id, dto, tokenSource);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            var updated = await _svc.GetByIdAsync(id);
            if (updated is not null)
            {
                Response.Headers["ETag"] = $"\"{updated.RowVersion}\"";
            }

            return NoContent();
        }

        /// <summary>
        /// Delete an existing agent. If `If-Match` header is provided the header value must match the current ETag (otherwise 412).
        /// Returns `204 NoContent` on success.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        public async Task<IActionResult> Delete([FromRoute] System.Guid id)
        {
            string? headerIfMatch = null;
            if (Request.Headers.TryGetValue("If-Match", out var ifMatchVals))
            {
                headerIfMatch = ifMatchVals.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(headerIfMatch))
                {
                    headerIfMatch = headerIfMatch.Trim();
                    if (headerIfMatch.StartsWith("W/")) headerIfMatch = headerIfMatch.Substring(2).Trim();
                    if (headerIfMatch.StartsWith("\"") && headerIfMatch.EndsWith("\"")) headerIfMatch = headerIfMatch[1..^1];
                }
            }

            try
            {
                await _svc.DeleteAsync(id, headerIfMatch);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                var traceId = Request.HttpContext?.Items != null && Request.HttpContext.Items.ContainsKey("CorrelationId") ? Request.HttpContext.Items["CorrelationId"] : Request.HttpContext?.TraceIdentifier;
                var body = new
                {
                    type = "https://example.com/probs/precondition-failed",
                    title = "Precondition Failed",
                    status = 412,
                    detail = "The provided ETag does not match the current resource state.",
                    instance = Request.Path.Value,
                    traceId,
                    timestamp = System.DateTime.UtcNow.ToString("o")
                };
                return new ObjectResult(body) { StatusCode = 412, ContentTypes = { "application/problem+json" } };
            }

            return NoContent();
        }

        /// <summary>
        /// Create a new agent. Responds with `201 Created` and an `ETag` header representing the created resource state.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] DTOs.CreateAgentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            _logger?.LogDebug("Create agent called with {Email}", dto.Email);
            var created = await _svc.CreateAsync(dto);
            Response.Headers["ETag"] = $"\"{created.RowVersion}\"";
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
    }
}
