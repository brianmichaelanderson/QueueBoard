using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace QueueBoard.Api.Services
{
    public class QueueService : IQueueService
    {
        private readonly QueueBoardDbContext _db;
        private readonly Microsoft.Extensions.Logging.ILogger<QueueService> _logger;
        private readonly Microsoft.AspNetCore.Http.IHttpContextAccessor _httpContextAccessor;

        public QueueService(QueueBoardDbContext db, Microsoft.Extensions.Logging.ILogger<QueueService> logger, Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _db.Queues.FindAsync(id);
            if (entity is null)
            {
                _logger?.LogInformation("Delete requested for missing queue {QueueId} — idempotent no-op", id);
                return;
            }

            // Capture context data for telemetry: traceId and user if available
            var httpContext = _httpContextAccessor.HttpContext;
            var traceId = httpContext?.Items != null && httpContext.Items.ContainsKey("CorrelationId") ? httpContext.Items["CorrelationId"]?.ToString() : httpContext?.TraceIdentifier;
            var user = httpContext?.User?.Identity?.IsAuthenticated == true ? httpContext.User.Identity?.Name : (httpContext?.Request?.Headers.ContainsKey("X-User") == true ? httpContext.Request.Headers["X-User"].ToString() : "anonymous");

            _db.Queues.Remove(entity);
            await _db.SaveChangesAsync();

            _logger?.LogInformation("Queue deleted: {QueueId} by {User} (traceId={TraceId})", id, user ?? "anonymous", traceId ?? "-" );
        }
    }
}
