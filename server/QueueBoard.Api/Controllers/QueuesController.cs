using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

            var items = await query
                .OrderBy(q => q.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(q => new QueueDto(q.Id, q.Name, q.Description, q.IsActive, q.CreatedAt))
                .ToListAsync();

            _logger?.LogInformation("Returned {Count} queues (total {Total}) for page {Page}", items.Count, total, page);
            return Ok(new { totalCount = total, page, pageSize, items });
        }

        /// <summary>
        /// Returns a single queue by id.
        /// </summary>
        /// <param name="id">The queue id (GUID).</param>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] System.Guid id)
        {
            _logger?.LogDebug("GetById queue {Id}", id);
            var dto = await _db.Queues
                .AsNoTracking()
                .Where(q => q.Id == id)
                .Select(q => new QueueDto(q.Id, q.Name, q.Description, q.IsActive, q.CreatedAt))
                .FirstOrDefaultAsync();

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
        public async Task<IActionResult> Create([FromBody] DTOs.CreateQueueDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            _logger?.LogDebug("Create called with Name='{Name}' Description='{Description}'", dto.Name, dto.Description);

            // Cross-field validation (TDD): ensure Name and Description are not identical
            var crossValidator = new Validators.QueueCrossFieldValidator();
            if (!crossValidator.Validate(new DTOs.QueueDto(System.Guid.Empty, dto.Name, dto.Description, dto.IsActive, System.DateTimeOffset.UtcNow), out var crossErrors))
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

            var result = new QueueDto(entity.Id, entity.Name, entity.Description, entity.IsActive, entity.CreatedAt);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, result);
        }

        /// <summary>
        /// Update an existing queue.
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update([FromRoute] System.Guid id, [FromBody] DTOs.CreateQueueDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            // Cross-field validation
            var crossValidator = new Validators.QueueCrossFieldValidator();
            if (!crossValidator.Validate(new DTOs.QueueDto(id, dto.Name, dto.Description, dto.IsActive, System.DateTimeOffset.UtcNow), out var crossErrors))
            {
                foreach (var e in crossErrors)
                {
                    ModelState.AddModelError(string.Empty, e);
                }
                return ValidationProblem(ModelState);
            }

            var entity = await _db.Queues.FindAsync(id);
            if (entity is null) return NotFound();

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.IsActive = dto.IsActive;
            entity.UpdatedAt = System.DateTimeOffset.UtcNow;

            await _db.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Delete an existing queue.
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] System.Guid id)
        {
            await _queueService.DeleteAsync(id);
            return NoContent();
        }
    }
}
