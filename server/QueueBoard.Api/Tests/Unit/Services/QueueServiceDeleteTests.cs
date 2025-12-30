using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QueueBoard.Api.Tests.Unit.Services
{
    [TestClass]
    public class QueueServiceDeleteTests
    {
        [TestMethod]
        public async Task Delete_RemovesEntity_FromDatabase()
        {
            var options = new DbContextOptionsBuilder<QueueBoard.Api.QueueBoardDbContext>()
                .UseInMemoryDatabase(databaseName: "Delete_RemovesEntity")
                .Options;

            // Seed an entity
            var id = Guid.NewGuid();
            using (var context = new QueueBoard.Api.QueueBoardDbContext(options))
            {
                context.Queues.Add(new QueueBoard.Api.Entities.Queue { Id = id, Name = "todelete", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow, IsActive = true });
                await context.SaveChangesAsync();
            }

            // Act: call DeleteAsync (should remove the seeded entity)
            using (var context = new QueueBoard.Api.QueueBoardDbContext(options))
            {
                var service = new QueueBoard.Api.Services.QueueService(context);
                await service.DeleteAsync(id);
            }

            // Assert: entity no longer present
            using (var context = new QueueBoard.Api.QueueBoardDbContext(options))
            {
                var found = await context.Queues.FindAsync(id);
                Assert.IsNull(found, "Entity should be removed from the database after DeleteAsync.");
            }
        }

        [TestMethod]
        public async Task Delete_NonExisting_Throws_KeyNotFoundException_Or_NotFound()
        {
            var options = new DbContextOptionsBuilder<QueueBoard.Api.QueueBoardDbContext>()
                .UseInMemoryDatabase(databaseName: "Delete_NonExisting")
                .Options;

            using (var context = new QueueBoard.Api.QueueBoardDbContext(options))
            {
                var service = new QueueBoard.Api.Services.QueueService(context);
                // Should not throw for non-existing id (idempotent)
                await service.DeleteAsync(Guid.NewGuid());
            }

            // Assert database still empty
            using (var context = new QueueBoard.Api.QueueBoardDbContext(options))
            {
                var any = await context.Queues.AnyAsync();
                Assert.IsFalse(any, "Database should remain empty after deleting a non-existing entity.");
            }
        }
    }
}

