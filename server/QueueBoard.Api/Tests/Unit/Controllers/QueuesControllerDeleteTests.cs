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
        public void Delete_WhenQueueMissing_ReturnsNotFound()
        {
            Assert.Inconclusive("Will be implemented after controller uses IQueueService; test should assert 204 on success and idempotent behavior.");
        }

        [TestMethod]
        public void Delete_Idempotent_RepeatedDeleteBehavior()
        {
            Assert.Inconclusive("Will be implemented after controller uses IQueueService; test should verify repeated deletes are idempotent and return 204.");
        }
    }
}
