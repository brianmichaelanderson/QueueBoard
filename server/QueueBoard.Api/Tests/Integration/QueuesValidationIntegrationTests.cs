using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QueueBoard.Api.Tests.Integration;

[TestCategory("Integration")]
[TestClass]
public class QueuesValidationIntegrationTests
{
    private static string ApiBaseUrl => System.Environment.GetEnvironmentVariable("TEST_API_BASE_URL") ?? "http://localhost:8080";

    [TestMethod]
    public async Task Post_InvalidQueue_ReturnsValidationProblemDetails()
    {
        using var client = new HttpClient { BaseAddress = new System.Uri(ApiBaseUrl) };

        // Missing required Name
        var payload = new
        {
            Description = "no name",
            IsActive = true
        };

        var resp = await client.PostAsJsonAsync("/queues", payload);

        Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, resp.StatusCode);
        Assert.IsTrue(resp.Content.Headers.ContentType.MediaType == "application/problem+json" || resp.Content.Headers.ContentType.MediaType == "application/json");

        var body = await resp.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Assert.IsTrue(body.TryGetProperty("errors", out var errors));
        Assert.IsTrue(body.TryGetProperty("traceId", out var traceId));
    }
}
