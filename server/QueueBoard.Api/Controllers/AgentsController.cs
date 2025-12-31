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
        private readonly Microsoft.Extensions.Logging.ILogger<AgentsController> _logger;

        public AgentsController(QueueBoardDbContext db, Microsoft.Extensions.Logging.ILogger<AgentsController> logger)
        {
            _db = db;
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
            var dtoRaw = await _db.Agents
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new { a.Id, a.FirstName, a.LastName, a.Email, a.IsActive, a.CreatedAt, a.UpdatedAt })
                .FirstOrDefaultAsync();

            if (dtoRaw is null)
            {
                _logger?.LogInformation("Agent {Id} not found", id);
                return NotFound();
            }
            var token = System.Convert.ToBase64String(System.BitConverter.GetBytes(dtoRaw.UpdatedAt.UtcTicks));
            var dto = new AgentDto(dtoRaw.Id, dtoRaw.FirstName, dtoRaw.LastName, dtoRaw.Email, dtoRaw.IsActive, dtoRaw.CreatedAt, token);
            Response.Headers["ETag"] = $"\"{token}\"";
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

            var providedTicks = System.BitConverter.ToInt64(tokenBytes, 0);
            if (providedTicks != entity.UpdatedAt.UtcTicks)
            {
                throw new Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException();
            }

            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
            entity.Email = dto.Email;
            entity.IsActive = dto.IsActive;
            entity.UpdatedAt = System.DateTimeOffset.UtcNow;

            await _db.SaveChangesAsync();

            var updatedToken = System.Convert.ToBase64String(System.BitConverter.GetBytes(entity.UpdatedAt.UtcTicks));
            Response.Headers["ETag"] = $"\"{updatedToken}\"";

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
            var request = Request;
            if (request != null && request.Headers != null && request.Headers.TryGetValue("If-Match", out var ifMatchVals))
            {
                var ifMatch = ifMatchVals.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(ifMatch))
                {
                    ifMatch = ifMatch.Trim();
                    if (ifMatch.StartsWith("W/")) ifMatch = ifMatch.Substring(2).Trim();
                    if (ifMatch.StartsWith("\"") && ifMatch.EndsWith("\"")) ifMatch = ifMatch[1..^1];

                    byte[] tokenBytes;
                    try
                    {
                        tokenBytes = System.Convert.FromBase64String(ifMatch);
                    }
                    catch
                    {
                        ModelState.AddModelError("If-Match", "If-Match header must contain a valid base64 token.");
                        return ValidationProblem(ModelState);
                    }

                    if (tokenBytes.Length < 8)
                    {
                        ModelState.AddModelError("If-Match", "If-Match token is invalid.");
                        return ValidationProblem(ModelState);
                    }

                    var providedTicks = System.BitConverter.ToInt64(tokenBytes, 0);

                    var entity = await _db.Agents.FindAsync(id);
                    if (entity is null) return NotFound();

                    if (providedTicks != entity.UpdatedAt.UtcTicks)
                    {
                        var traceId = request.HttpContext?.Items != null && request.HttpContext.Items.ContainsKey("CorrelationId") ? request.HttpContext.Items["CorrelationId"] : request.HttpContext?.TraceIdentifier;
                        var body = new
                        {
                            type = "https://example.com/probs/precondition-failed",
                            title = "Precondition Failed",
                            status = 412,
                            detail = "The provided ETag does not match the current resource state.",
                            instance = request.Path.Value,
                            traceId,
                            timestamp = System.DateTime.UtcNow.ToString("o")
                        };
                        return new ObjectResult(body) { StatusCode = 412, ContentTypes = { "application/problem+json" } };
                    }
                }
            }

            var existing = await _db.Agents.FindAsync(id);
            if (existing is null)
            {
                // idempotent delete
                return NoContent();
            }

            _db.Agents.Remove(existing);
            await _db.SaveChangesAsync();
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

            var entity = new Entities.Agent
            {
                Id = System.Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                IsActive = dto.IsActive,
                CreatedAt = System.DateTimeOffset.UtcNow,
                UpdatedAt = System.DateTimeOffset.UtcNow
            };

            _db.Agents.Add(entity);
            await _db.SaveChangesAsync();

            var token = System.Convert.ToBase64String(System.BitConverter.GetBytes(entity.UpdatedAt.UtcTicks));
            var result = new DTOs.AgentDto(entity.Id, entity.FirstName, entity.LastName, entity.Email, entity.IsActive, entity.CreatedAt, token);
            Response.Headers["ETag"] = $"\"{token}\"";
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, result);
        }
    }
}
