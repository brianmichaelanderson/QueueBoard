using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QueueBoard.Api.Tests.Integration
{
    [TestClass]
    public class QueuesPutCrossFieldIntegrationTests
    {
        [TestMethod]
        public async Task Put_InvalidQueue_NameEqualsDescription_ReturnsValidationProblemDetails()
        {
            var client = new HttpClient { BaseAddress = new System.Uri("http://localhost:8080") };

            // Create a valid queue first
            var createPayload = new { name = "Initial", description = "Initial description", isActive = true };
            var createContent = new StringContent(JsonSerializer.Serialize(createPayload), Encoding.UTF8, "application/json");
            var createResp = await client.PostAsync("/queues", createContent);
            Assert.AreEqual(System.Net.HttpStatusCode.Created, createResp.StatusCode);

            var createdJson = await createResp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(createdJson);
            var id = doc.RootElement.GetProperty("id").GetGuid();

            // Attempt an invalid update where Name == Description
            var updatePayload = new { name = "Same", description = "Same", isActive = true };
            var updateContent = new StringContent(JsonSerializer.Serialize(updatePayload), Encoding.UTF8, "application/json");
            var updateResp = await client.PutAsync($"/queues/{id}", updateContent);

            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, updateResp.StatusCode);
            Assert.AreEqual("application/problem+json", updateResp.Content.Headers.ContentType?.MediaType);

            var json = await updateResp.Content.ReadAsStringAsync();
            Assert.IsTrue(json.Contains("Name and Description must not be identical."));
        }
    }
}
