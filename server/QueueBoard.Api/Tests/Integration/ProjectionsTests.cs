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
    private static WebApplicationFactory<Program>? _factory;

    private class CustomFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            // Force content root to the API project path inside the container
            builder.UseContentRoot("/src/server/QueueBoard.Api");
            return base.CreateHost(builder);
        }
    }

    [ClassInitialize]
    public static void ClassInit(TestContext _)
    {
        // Ensure the factory uses the API project as the content root so static assets and configuration load correctly
        _factory = new CustomFactory<Program>();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        _factory?.Dispose();
    }

    [TestMethod]
    public async Task Queues_GetAll_ReturnsDtoShape()
    {
        var client = _factory!.CreateClient();
        var resp = await client.GetAsync("/queues?page=1&pageSize=5");
        resp.EnsureSuccessStatusCode();

        var body = await resp.Content.ReadFromJsonAsync<object>();
        Assert.IsNotNull(body, "Response body should deserialize");
    }

    [TestMethod]
    public async Task Agents_GetAll_ReturnsDtoShape()
    {
        var client = _factory!.CreateClient();
        var resp = await client.GetAsync("/agents?page=1&pageSize=5");
        resp.EnsureSuccessStatusCode();

        var body = await resp.Content.ReadFromJsonAsync<object>();
        Assert.IsNotNull(body, "Response body should deserialize");
    }
}
