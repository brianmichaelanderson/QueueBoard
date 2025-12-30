using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueueBoard.Api.Tests.Integration.TestHelpers;

namespace QueueBoard.Api.Tests.Integration;

[TestClass]
public class QueuesConcurrencyIntegrationTests
{
    private static string ApiBaseUrl => System.Environment.GetEnvironmentVariable("TEST_API_BASE_URL") ?? "http://localhost:8080";

    [TestMethod]
    public async Task ConcurrentUpdates_OneShouldReturnConflict_WithProblemDetails()
    {
        await ReadinessHelper.WaitForApiAsync(ApiBaseUrl);

        using var client = new System.Net.Http.HttpClient { BaseAddress = new System.Uri(ApiBaseUrl) };

        var create = new { Name = "concurrency-queue", Description = "for concurrency test", IsActive = true };
        var createResp = await client.PostAsJsonAsync("/queues", create);
        createResp.EnsureSuccessStatusCode();

        var created = await createResp.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetGuid();

        // Try several attempts to provoke a concurrency conflict by issuing parallel updates
        const int maxAttempts = 5;
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            var payload1 = new { Name = $"concurrency-{attempt}-a", Description = "a", IsActive = true };
            var payload2 = new { Name = $"concurrency-{attempt}-b", Description = "b", IsActive = true };

            var t1 = client.PutAsJsonAsync($"/queues/{id}", payload1);
            var t2 = client.PutAsJsonAsync($"/queues/{id}", payload2);

            await Task.WhenAll(t1, t2);

            var r1 = await t1;
            var r2 = await t2;

            // Check if any returned 409 with application/problem+json
            foreach (var resp in new[] { r1, r2 })
            {
                if (resp.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    Assert.IsTrue(resp.Content.Headers.ContentType != null && resp.Content.Headers.ContentType.MediaType == "application/problem+json", "Conflict response should be ProblemDetails");
                    var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
                    Assert.IsTrue(doc.RootElement.TryGetProperty("status", out var statusEl) && statusEl.GetInt32() == 409, "ProblemDetails should include status=409");
                    return; // test passes
                }
            }

            // small delay before next attempt to vary timing
            await Task.Delay(200);
        }

        Assert.Fail($"Did not observe a 409 Conflict after {maxAttempts} concurrent attempts; concurrency behavior may not be reproducible under these test conditions.");
    }
}
