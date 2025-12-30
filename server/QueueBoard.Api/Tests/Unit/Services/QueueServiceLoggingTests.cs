using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace QueueBoard.Api.Tests.Unit.Services
{
    [TestClass]
    public class QueueServiceLoggingTests
    {
        [TestMethod]
        public async Task DeleteAsync_LogsDeletion_WithTraceIdAndUser()
        {
            var options = new DbContextOptionsBuilder<QueueBoard.Api.QueueBoardDbContext>()
                .UseInMemoryDatabase(databaseName: "Delete_LogsDeletion")
                .Options;

            var id = Guid.NewGuid();
            using (var seed = new QueueBoard.Api.QueueBoardDbContext(options))
            {
                seed.Queues.Add(new QueueBoard.Api.Entities.Queue { Id = id, Name = "todelete", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow, IsActive = true });
                await seed.SaveChangesAsync();
            }

            using (var context = new QueueBoard.Api.QueueBoardDbContext(options))
            {
                var mockLogger = new Mock<ILogger<QueueBoard.Api.Services.QueueService>>();

                // Arrange HttpContext with trace id and X-User header
                var httpCtx = new DefaultHttpContext();
                httpCtx.Items["CorrelationId"] = "|trace-id-test|";
                httpCtx.Request.Headers["X-User"] = "unittest-user";
                var httpAccessor = new HttpContextAccessor { HttpContext = httpCtx };

                // Create service with mocked logger and invoke delete
                var service = new QueueBoard.Api.Services.QueueService(context, mockLogger.Object, httpAccessor);

                // Act
                await service.DeleteAsync(id);

                // Inspect invocations on the mock to find a Log call that contains our expected tokens
                var found = false;
                foreach (var inv in mockLogger.Invocations)
                {
                    if (inv.Method.Name == "Log")
                    {
                        var state = inv.Arguments.Count > 2 ? inv.Arguments[2] : null;
                        var msg = state?.ToString() ?? string.Empty;
                        if (msg.Contains(id.ToString()) && msg.Contains("unittest-user") && msg.Contains("trace-id-test"))
                        {
                            found = true; break;
                        }
                    }
                }

                Assert.IsTrue(found, "Expected the logger to receive a message containing queue id, user, and trace id.");
            }
        }
    }
}
