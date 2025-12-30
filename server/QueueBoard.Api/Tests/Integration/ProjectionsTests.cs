using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;

namespace QueueBoard.Api.Tests.Integration;

[TestClass]
public class ProjectionsTests
{
    // Integration tests call the running API service (compose) at localhost:8080 when executed inside the container.
    private static string ApiBaseUrl => System.Environment.GetEnvironmentVariable("TEST_API_BASE_URL") ?? "http://localhost:8080";

    [TestMethod]
    public async Task Queues_GetAll_ReturnsDtoShape()
    {
        using var client = new System.Net.Http.HttpClient { BaseAddress = new System.Uri(ApiBaseUrl) };
        var resp = await client.GetAsync("/queues?page=1&pageSize=5");
        resp.EnsureSuccessStatusCode();

        var body = await resp.Content.ReadFromJsonAsync<object>();
        Assert.IsNotNull(body, "Response body should deserialize");
    }

    [TestMethod]
    public async Task Agents_GetAll_ReturnsDtoShape()
    {
        using var client = new System.Net.Http.HttpClient { BaseAddress = new System.Uri(ApiBaseUrl) };
        var resp = await client.GetAsync("/agents?page=1&pageSize=5");
        resp.EnsureSuccessStatusCode();

        var body = await resp.Content.ReadFromJsonAsync<object>();
        Assert.IsNotNull(body, "Response body should deserialize");
    }
}
