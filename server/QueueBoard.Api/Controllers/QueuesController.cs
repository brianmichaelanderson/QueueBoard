using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using QueueBoard.Api.DTOs;

namespace QueueBoard.Api.Controllers
{
    /// <summary>
    /// Controller for managing queues: list and fetch by id.
    /// </summary>
    [ApiController]
    [Route("queues")]
    public class QueuesController : ControllerBase
    {
        private readonly QueueBoardDbContext _db;
        private readonly Microsoft.Extensions.Logging.ILogger<QueuesController> _logger;
        private readonly Services.IQueueService _queueService;

        public QueuesController(QueueBoardDbContext db, Services.IQueueService queueService, Microsoft.Extensions.Logging.ILogger<QueuesController> logger)
        {
            _db = db;
            _queueService = queueService;
            _logger = logger;
        }

        /// <summary>
        /// Returns a paginated list of queues. Supports optional search, paging and page size.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 25)
        {
            _logger?.LogDebug("GetAll queues {Search} page {Page} size {PageSize}", search, page, pageSize);
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 25;

            var query = _db.Queues.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                query = query.Where(q => q.Name.Contains(s) || (q.Description != null && q.Description.Contains(s)));
            }

            var total = await query.CountAsync();

            var itemsRaw = await query
                .OrderBy(q => q.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(q => new { q.Id, q.Name, q.Description, q.IsActive, q.CreatedAt, q.UpdatedAt })
                .ToListAsync();

            var items = itemsRaw
                .Select(q => new QueueDto(q.Id, q.Name, q.Description, q.IsActive, q.CreatedAt, System.Convert.ToBase64String(System.BitConverter.GetBytes(q.UpdatedAt.UtcTicks))))
                .ToList();

            _logger?.LogInformation("Returned {Count} queues (total {Total}) for page {Page}", items.Count, total, page);
            return Ok(new { totalCount = total, page, pageSize, items });
        }

        /// <summary>
        /// Returns a single queue by id.
        /// </summary>
        /// <param name="id">The queue id (GUID).</param>
        [HttpGet("{id:guid}")]
        /// <remarks>
        /// Returns the queue DTO and an `ETag` response header containing a base64 token representing the resource state.
        /// Clients can use this `ETag` value in an `If-Match` header when calling `PUT` or `DELETE` to perform optimistic concurrency checks.
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] System.Guid id)
        {
            _logger?.LogDebug("GetById queue {Id}", id);
            var dtoRaw = await _db.Queues
                .AsNoTracking()
                .Where(q => q.Id == id)
                .Select(q => new { q.Id, q.Name, q.Description, q.IsActive, q.CreatedAt, q.UpdatedAt })
                .FirstOrDefaultAsync();

            QueueDto? dto = null;
            if (dtoRaw is not null)
            {
                var token = System.Convert.ToBase64String(System.BitConverter.GetBytes(dtoRaw.UpdatedAt.UtcTicks));
                dto = new QueueDto(dtoRaw.Id, dtoRaw.Name, dtoRaw.Description, dtoRaw.IsActive, dtoRaw.CreatedAt, token);
                Response.Headers["ETag"] = $"\"{token}\"";
            }

            if (dto is null)
            {
                _logger?.LogInformation("Queue {Id} not found", id);
                return NotFound();
            }
            _logger?.LogInformation("Returned queue {Id}", id);
            return Ok(dto);
        }

        /// <summary>
        /// Create a new queue.
        /// </summary>
        [HttpPost]
        /// <summary>
        /// Create a new queue. Responds with `201 Created` and an `ETag` header representing the created resource state.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] DTOs.CreateQueueDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            _logger?.LogDebug("Create called with Name='{Name}' Description='{Description}'", dto.Name, dto.Description);

            // Cross-field validation (TDD): ensure Name and Description are not identical
            var crossValidator = new Validators.QueueCrossFieldValidator();
            if (!crossValidator.Validate(new DTOs.QueueDto(System.Guid.Empty, dto.Name, dto.Description, dto.IsActive, System.DateTimeOffset.UtcNow, null), out var crossErrors))
            {
                foreach (var e in crossErrors)
                {
                    ModelState.AddModelError(string.Empty, e);
                }
                return ValidationProblem(ModelState);
            }

            var entity = new Entities.Queue
            {
                Id = System.Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedAt = System.DateTimeOffset.UtcNow,
                UpdatedAt = System.DateTimeOffset.UtcNow
            };

            _db.Queues.Add(entity);
            await _db.SaveChangesAsync();

            var newToken = System.Convert.ToBase64String(System.BitConverter.GetBytes(entity.UpdatedAt.UtcTicks));
            var result = new QueueDto(entity.Id, entity.Name, entity.Description, entity.IsActive, entity.CreatedAt, newToken);
            Response.Headers["ETag"] = $"\"{newToken}\"";
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, result);
        }

        /// <summary>
        /// Update an existing queue.
        /// </summary>
        [HttpPut("{id:guid}")]
        /// <summary>
        /// Update an existing queue. Requires either `RowVersion` in the body or an `If-Match` header with the ETag provided by a previous GET/POST.
        /// On stale token, returns `409 Conflict` (mapped via problem details).
        /// </summary>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        public async Task<IActionResult> Update([FromRoute] System.Guid id, [FromBody] DTOs.UpdateQueueDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            // Cross-field validation
            var crossValidator = new Validators.QueueCrossFieldValidator();
            if (!crossValidator.Validate(new DTOs.QueueDto(id, dto.Name, dto.Description, dto.IsActive, System.DateTimeOffset.UtcNow, null), out var crossErrors))
            {
                foreach (var e in crossErrors)
                {
                    ModelState.AddModelError(string.Empty, e);
                }
                return ValidationProblem(ModelState);
            }

            var entity = await _db.Queues.FindAsync(id);
            if (entity is null) return NotFound();

            // Accept RowVersion from body or If-Match header (ETag)
            string? headerIfMatch = null;
            if (Request.Headers.TryGetValue("If-Match", out var ifMatchVals))
            {
                headerIfMatch = ifMatchVals.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(headerIfMatch))
                {
                    // strip weak prefix and surrounding quotes
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

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.IsActive = dto.IsActive;
            entity.UpdatedAt = System.DateTimeOffset.UtcNow;

            await _db.SaveChangesAsync();

            // return new ETag for updated resource
            var updatedToken = System.Convert.ToBase64String(System.BitConverter.GetBytes(entity.UpdatedAt.UtcTicks));
            Response.Headers["ETag"] = $"\"{updatedToken}\"";

            return NoContent();
        }

        /// <summary>
        /// Delete an existing queue.
        /// </summary>
        [HttpDelete("{id:guid}")]
        /// <summary>
        /// Delete an existing queue. If `If-Match` header is provided the header value must match the current ETag (otherwise 412).
        /// Returns `204 NoContent` on success.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        public async Task<IActionResult> Delete([FromRoute] System.Guid id)
        {
            // If client provided an If-Match header, validate it against the current token
            var request = Request;
            if (request != null && request.Headers != null && request.Headers.TryGetValue("If-Match", out var ifMatchVals))
            {
                var ifMatch = ifMatchVals.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(ifMatch))
                {
                    // normalize: remove weak indicator and surrounding quotes
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

                    var entity = await _db.Queues.FindAsync(id);
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

            await _queueService.DeleteAsync(id);
            return NoContent();
        }
    }
}
