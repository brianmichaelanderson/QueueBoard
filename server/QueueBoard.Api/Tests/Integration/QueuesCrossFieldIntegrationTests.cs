using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QueueBoard.Api.Tests.Integration
{
    [TestClass]
    public class QueuesCrossFieldIntegrationTests
    {
        // Respect TEST_API_BASE_URL when running inside containers
        private static string ApiBaseUrl => System.Environment.GetEnvironmentVariable("TEST_API_BASE_URL") ?? "http://localhost:8080";
        [TestMethod]
        public async Task Post_InvalidQueue_NameEqualsDescription_ReturnsValidationProblemDetails()
        {
            var client = new HttpClient { BaseAddress = new System.Uri(ApiBaseUrl) };

            var payload = new
            {
                name = "Same",
                description = "Same",
                isActive = true
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await client.PostAsync("/queues", content);

            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, resp.StatusCode);
            Assert.AreEqual("application/problem+json", resp.Content.Headers.ContentType?.MediaType);

            var json = await resp.Content.ReadAsStringAsync();
            Assert.IsTrue(json.Contains("Name and Description must not be identical."));
        }
    }
}
