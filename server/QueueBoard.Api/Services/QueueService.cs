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
            // TDD: Not implemented yet — tests will drive the implementation.
            throw new NotImplementedException();
        }
    }
}
