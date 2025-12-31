using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QueueBoard.Api.Tests.Integration
{
    [TestClass]
    public class AgentsIntegrationTests
    {
        [TestMethod]
        public void Create_Get_Update_Delete_HappyPath()
        {
            // Arrange
            // TODO: use SDK/container-based test runner and reset-db.sh to ensure clean DB

            // Act
            // TODO: POST /agents -> GET -> PUT -> DELETE sequence

            // Assert
            Assert.Inconclusive("Scaffold: implement integration happy-path test for agents using containerized API");
        }

        [TestMethod]
        public void Update_With_Stale_ETag_Returns_409()
        {
            // Arrange
            // TODO: create agent, capture ETag, mutate record externally to change RowVersion

            // Act
            // TODO: attempt PUT with stale If-Match and expect 409

            // Assert
            Assert.Inconclusive("Scaffold: implement integration test to verify concurrency conflicts return 409");
        }
    }
}
