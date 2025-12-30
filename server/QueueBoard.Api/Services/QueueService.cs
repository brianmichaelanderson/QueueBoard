using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace QueueBoard.Api.Services
{
    public class QueueService : IQueueService
    {
        private readonly QueueBoardDbContext _db;

        public QueueService(QueueBoardDbContext db)
        {
            _db = db;
        }

        public async Task DeleteAsync(Guid id)
        {
            // Hard-delete semantics: remove the entity if it exists.
            var entity = await _db.Queues.FindAsync(id);
            if (entity is null)
            {
                // Idempotent: deleting a missing resource is a no-op.
                return;
            }

            _db.Queues.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
