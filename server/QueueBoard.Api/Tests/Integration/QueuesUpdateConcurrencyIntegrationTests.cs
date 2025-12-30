using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueueBoard.Api.Tests.Integration.TestHelpers;

namespace QueueBoard.Api.Tests.Integration;

[TestClass]
public class QueuesUpdateConcurrencyIntegrationTests
{
    private static string ApiBaseUrl => System.Environment.GetEnvironmentVariable("TEST_API_BASE_URL") ?? "http://localhost:8080";

    [TestMethod]
    public async Task Update_WithStaleRowVersion_ReturnsConflict_WithProblemDetails()
    {
        await ReadinessHelper.WaitForApiAsync(ApiBaseUrl);

        using var client = new System.Net.Http.HttpClient { BaseAddress = new System.Uri(ApiBaseUrl) };

        var create = new { Name = "concurrency-deterministic-queue", Description = "for deterministic concurrency test", IsActive = true };
        var createResp = await client.PostAsJsonAsync("/queues", create);
        createResp.EnsureSuccessStatusCode();

        var created = await createResp.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetGuid();

        // Read the queue to obtain the RowVersion token (expected as base64 string in 'rowVersion')
        var getResp = await client.GetAsync($"/queues/{id}");
        getResp.EnsureSuccessStatusCode();
        var rawGet = await getResp.Content.ReadAsStringAsync();
        System.Console.WriteLine("GET response raw: " + rawGet);
        var body = JsonDocument.Parse(rawGet).RootElement;

        Assert.IsTrue(body.TryGetProperty("rowVersion", out var rvEl), "Response must include 'rowVersion' (base64 string) for concurrency tests");
        var rowVersion = rvEl.GetString();
        Assert.IsFalse(string.IsNullOrWhiteSpace(rowVersion));

        // First update with current RowVersion should succeed
        var update1 = new { Name = "concurrency-deterministic-queue-updated", Description = "first", IsActive = true, RowVersion = rowVersion };
        var u1 = await client.PutAsJsonAsync($"/queues/{id}", update1);
        u1.EnsureSuccessStatusCode();

        // Second update using the *old* (stale) rowVersion should produce a 409 Conflict with ProblemDetails
        var updateStale = new { Name = "concurrency-deterministic-queue-updated-2", Description = "stale", IsActive = true, RowVersion = rowVersion };
        var staleResp = await client.PutAsJsonAsync($"/queues/{id}", updateStale);

        Assert.AreEqual(System.Net.HttpStatusCode.Conflict, staleResp.StatusCode, "Stale update should return 409 Conflict");
        Assert.IsTrue(staleResp.Content.Headers.ContentType != null && staleResp.Content.Headers.ContentType.MediaType == "application/problem+json", "Conflict response should be ProblemDetails");

        var doc = JsonDocument.Parse(await staleResp.Content.ReadAsStringAsync());
        Assert.IsTrue(doc.RootElement.TryGetProperty("status", out var statusEl) && statusEl.GetInt32() == 409, "ProblemDetails should include status=409");
    }
}
