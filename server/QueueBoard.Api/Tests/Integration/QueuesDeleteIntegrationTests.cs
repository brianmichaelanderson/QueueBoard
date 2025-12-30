using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueueBoard.Api.Tests.Integration.TestHelpers;

namespace QueueBoard.Api.Tests.Integration;

[TestClass]
public class QueuesDeleteIntegrationTests
{
    private static string ApiBaseUrl => System.Environment.GetEnvironmentVariable("TEST_API_BASE_URL") ?? "http://localhost:8080";

    [TestMethod]
    public async Task Create_Delete_Get_ReturnsNotFound()
    {
        await ReadinessHelper.WaitForApiAsync(ApiBaseUrl);

        using var client = new System.Net.Http.HttpClient { BaseAddress = new System.Uri(ApiBaseUrl) };

        // Create a queue
        var create = new { Name = "it-test-queue", Description = "for integration delete test", IsActive = true };
        var createResp = await client.PostAsJsonAsync("/queues", create);
        createResp.EnsureSuccessStatusCode();

        var created = await createResp.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        var id = created.GetProperty("id").GetGuid();

        // Delete it
        var delResp = await client.DeleteAsync($"/queues/{id}");
        Assert.IsTrue(delResp.IsSuccessStatusCode, "Delete should succeed");

        // Get it -> expect 404
        var getResp = await client.GetAsync($"/queues/{id}");
        Assert.AreEqual(System.Net.HttpStatusCode.NotFound, getResp.StatusCode);
    }

    [TestMethod]
    public async Task Delete_Idempotent_RepeatedDelete_ReturnsNoContent()
    {
        await ReadinessHelper.WaitForApiAsync(ApiBaseUrl);

        using var client = new System.Net.Http.HttpClient { BaseAddress = new System.Uri(ApiBaseUrl) };

        // Create a queue
        var create = new { Name = "it-test-queue-2", Description = "for idempotent test", IsActive = true };
        var createResp = await client.PostAsJsonAsync("/queues", create);
        createResp.EnsureSuccessStatusCode();

        var created = await createResp.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        var id = created.GetProperty("id").GetGuid();

        // First delete
        var del1 = await client.DeleteAsync($"/queues/{id}");
        Assert.AreEqual(System.Net.HttpStatusCode.NoContent, del1.StatusCode);

        // Second delete (idempotent) — expect 204 as per plan
        var del2 = await client.DeleteAsync($"/queues/{id}");
        Assert.AreEqual(System.Net.HttpStatusCode.NoContent, del2.StatusCode);
    }
}
