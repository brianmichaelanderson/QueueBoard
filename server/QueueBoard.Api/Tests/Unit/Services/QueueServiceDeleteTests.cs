using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using QueueBoard.Api.Tests.Unit.Helpers;

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
                var httpCtx = new DefaultHttpContext();
                httpCtx.Items["CorrelationId"] = "|trace-id-test|";
                httpCtx.Request.Headers["X-User"] = "unittest-user";
                var httpAccessor = new HttpContextAccessor { HttpContext = httpCtx };

                var logger = new TestLogger<QueueBoard.Api.Services.QueueService>();
                var service = new QueueBoard.Api.Services.QueueService(context, logger, httpAccessor);
                await service.DeleteAsync(id);

                // Assert logger recorded a deletion message
                Assert.IsTrue(logger.Messages.Count > 0, "Expected at least one log message for delete.");
                var joined = string.Join("\n", logger.Messages);
                Assert.IsTrue(joined.Contains(id.ToString()), "Log should contain the queue id.");
                Assert.IsTrue(joined.Contains("unittest-user"), "Log should contain the user identity/header.");
                Assert.IsTrue(joined.Contains("trace-id-test"), "Log should contain the trace id.");
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
                var httpCtx = new DefaultHttpContext();
                httpCtx.Items["CorrelationId"] = "|trace-id-test|";
                httpCtx.Request.Headers["X-User"] = "unittest-user";
                var httpAccessor = new HttpContextAccessor { HttpContext = httpCtx };

                var logger = new TestLogger<QueueBoard.Api.Services.QueueService>();
                var service = new QueueBoard.Api.Services.QueueService(context, logger, httpAccessor);
                // Should not throw for non-existing id (idempotent)
                await service.DeleteAsync(Guid.NewGuid());

                // Should have logged idempotent no-op
                Assert.IsTrue(logger.Messages.Count > 0, "Expected log messages for idempotent delete.");
                var joined2 = string.Join("\n", logger.Messages);
                Assert.IsTrue(joined2.Contains("idempotent"), "Log should indicate idempotent no-op.");
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

