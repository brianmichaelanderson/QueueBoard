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

            // Create service backed by in-memory context; DeleteAsync not implemented yet — this test should fail (TDD)
            using (var context = new QueueBoard.Api.QueueBoardDbContext(options))
            {
                var service = new QueueBoard.Api.Services.QueueService(context);
                await Assert.ThrowsExceptionAsync<NotImplementedException>(async () => await service.DeleteAsync(id));
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
                await Assert.ThrowsExceptionAsync<NotImplementedException>(async () => await service.DeleteAsync(Guid.NewGuid()));
            }
        }
    }
}

