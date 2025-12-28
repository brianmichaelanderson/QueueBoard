using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public AgentsController(QueueBoardDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Returns a paginated list of agents. Supports optional search, paging and page size.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 25)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 25;

            var query = _db.Agents.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                query = query.Where(a => a.FirstName.Contains(s) || a.LastName.Contains(s) || a.Email.Contains(s));
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(a => a.LastName).ThenBy(a => a.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AgentDto(a.Id, a.FirstName, a.LastName, a.Email, a.IsActive, a.CreatedAt))
                .ToListAsync();

            return Ok(new { totalCount = total, page, pageSize, items });
        }

        /// <summary>
        /// Returns a single agent by id.
        /// </summary>
        /// <param name="id">The agent id (GUID).</param>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] System.Guid id)
        {
            var dto = await _db.Agents
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new AgentDto(a.Id, a.FirstName, a.LastName, a.Email, a.IsActive, a.CreatedAt))
                .FirstOrDefaultAsync();

            if (dto is null) return NotFound();
            return Ok(dto);
        }
    }
}
