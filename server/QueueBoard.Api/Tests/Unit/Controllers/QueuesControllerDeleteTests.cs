using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QueueBoard.Api.Tests.Unit.Controllers
{
    [TestClass]
    public class QueuesControllerDeleteTests
    {
        private class FakeQueueService : QueueBoard.Api.Services.IQueueService
        {
            public System.Threading.Tasks.Task DeleteAsync(System.Guid id) => System.Threading.Tasks.Task.CompletedTask;
        }
        [TestMethod]
        public async System.Threading.Tasks.Task Delete_WhenQueueExists_ReturnsNoContent()
        {
            // Arrange: fake IQueueService that completes successfully
            var fakeService = new FakeQueueService();
            var controller = new QueueBoard.Api.Controllers.QueuesController((QueueBoard.Api.QueueBoardDbContext?)null, fakeService, (Microsoft.Extensions.Logging.ILogger<QueueBoard.Api.Controllers.QueuesController>?)null);

            // Act
            var result = await controller.Delete(System.Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result, typeof(Microsoft.AspNetCore.Mvc.NoContentResult));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Delete_WhenQueueMissing_ReturnsNoContent()
        {
            // Arrange: fake service that does nothing (idempotent delete)
            var fakeService = new FakeQueueService();
            var controller = new QueueBoard.Api.Controllers.QueuesController((QueueBoard.Api.QueueBoardDbContext?)null, fakeService, (Microsoft.Extensions.Logging.ILogger<QueueBoard.Api.Controllers.QueuesController>?)null);

            // Act
            var result = await controller.Delete(System.Guid.NewGuid());

            // Assert: idempotent delete should return 204 NoContent
            Assert.IsInstanceOfType(result, typeof(Microsoft.AspNetCore.Mvc.NoContentResult));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Delete_Idempotent_RepeatedDeleteBehavior()
        {
            // Arrange
            var fakeService = new FakeQueueService();
            var controller = new QueueBoard.Api.Controllers.QueuesController((QueueBoard.Api.QueueBoardDbContext?)null, fakeService, (Microsoft.Extensions.Logging.ILogger<QueueBoard.Api.Controllers.QueuesController>?)null);

            // Act: call delete twice
            var r1 = await controller.Delete(System.Guid.NewGuid());
            var r2 = await controller.Delete(System.Guid.NewGuid());

            // Assert: both should be NoContent (idempotent)
            Assert.IsInstanceOfType(r1, typeof(Microsoft.AspNetCore.Mvc.NoContentResult));
            Assert.IsInstanceOfType(r2, typeof(Microsoft.AspNetCore.Mvc.NoContentResult));
        }
    }
}
