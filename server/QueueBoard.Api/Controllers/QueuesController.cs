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

        public QueuesController(QueueBoardDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Returns a paginated list of queues. Supports optional search, paging and page size.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 25)
        {
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

            return Ok(new { totalCount = total, page, pageSize, items });
        }

        /// <summary>
        /// Returns a single queue by id.
        /// </summary>
        /// <param name="id">The queue id (GUID).</param>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] System.Guid id)
        {
            var dto = await _db.Queues
                .AsNoTracking()
                .Where(q => q.Id == id)
                .Select(q => new QueueDto(q.Id, q.Name, q.Description, q.IsActive, q.CreatedAt))
                .FirstOrDefaultAsync();

            if (dto is null) return NotFound();
            return Ok(dto);
        }
    }
}
