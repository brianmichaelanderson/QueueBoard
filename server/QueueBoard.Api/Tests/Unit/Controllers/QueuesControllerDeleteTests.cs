using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QueueBoard.Api.Tests.Unit.Controllers
{
    [TestClass]
    public class QueuesControllerDeleteTests
    {
        [TestMethod]
        public async System.Threading.Tasks.Task Delete_WhenQueueExists_ReturnsNoContent()
        {
            var controller = new QueueBoard.Api.Controllers.QueuesController((QueueBoard.Api.QueueBoardDbContext?)null, (Microsoft.Extensions.Logging.ILogger<QueueBoard.Api.Controllers.QueuesController>?)null);

            try
            {
                var result = await controller.Delete(System.Guid.NewGuid());
                Assert.IsInstanceOfType(result, typeof(Microsoft.AspNetCore.Mvc.NoContentResult));
            }
            catch (System.NotImplementedException)
            {
                Assert.Fail("Delete endpoint not implemented yet — expected test to fail until implementation (TDD).");
            }
        }

        [TestMethod]
        public void Delete_WhenQueueMissing_ReturnsNotFound()
        {
            Assert.Inconclusive("Implement after delete semantics are implemented: expect 204 on success, idempotent deletes return 204.");
        }

        [TestMethod]
        public void Delete_Idempotent_RepeatedDeleteBehavior()
        {
            Assert.Inconclusive("Implement after delete semantics are implemented: assert repeated deletes are idempotent and return 204.");
        }
    }
}
