using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace QueueBoard.Api.Controllers
{
    /// <summary>
    /// Simple health endpoint that reports database connectivity and timestamp.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly QueueBoardDbContext _db;

        public HealthController(QueueBoardDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Returns liveness/readiness status and a short set of checks.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var dbOk = await _db.Database.CanConnectAsync();
            var payload = new
            {
                status = dbOk ? "Healthy" : "Unhealthy",
                checks = new { database = dbOk ? "Healthy" : "Unhealthy" },
                timestamp = DateTime.UtcNow.ToString("o")
            };

            return dbOk
                ? Ok(payload)
                : StatusCode(StatusCodes.Status503ServiceUnavailable, payload);
        }
    }
}
