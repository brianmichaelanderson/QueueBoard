using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QueueBoard.Api.Tests.Unit;

[TestClass]
public class CustomProblemDetailsFactoryTests
{
    private class TestHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Production;
        public string ApplicationName { get; set; } = "QueueBoard.Api";
        public string ContentRootPath { get; set; } = "/src/server/QueueBoard.Api";
        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } = new Microsoft.Extensions.FileProviders.NullFileProvider();
    }

    [TestMethod]
    public void CreateProblemDetails_OmitsDetail_InProduction()
    {
        var context = new DefaultHttpContext();
        // attach a minimal IHostEnvironment indicating Production
        var services = new ServiceCollection();
        services.AddSingleton<IHostEnvironment>(new TestHostEnvironment { EnvironmentName = Environments.Production });
        context.RequestServices = services.BuildServiceProvider();

        context.Items["CorrelationId"] = "cid-prod-1";

        var factory = new QueueBoard.Api.Services.CustomProblemDetailsFactory();
        var pd = factory.CreateProblemDetails(context, 500, "Err", "https://example.com/probs/err", "sensitive detail should be hidden", "/x");

        // In production we expect the factory to omit or redact the detail field
        Assert.IsNull(pd.Detail, "Detail should be null in production to avoid leaking sensitive information.");
        Assert.IsTrue(pd.Extensions.ContainsKey("traceId"));
        Assert.AreEqual("cid-prod-1", pd.Extensions["traceId"]);
    }

    [TestMethod]
    public void CreateProblemDetails_PreservesDetail_InDevelopment()
    {
        var context = new DefaultHttpContext();
        // attach a minimal IHostEnvironment indicating Development
        var services = new ServiceCollection();
        services.AddSingleton<IHostEnvironment>(new TestHostEnvironment { EnvironmentName = Environments.Development });
        context.RequestServices = services.BuildServiceProvider();

        context.Items["CorrelationId"] = "cid-dev-1";

        var factory = new QueueBoard.Api.Services.CustomProblemDetailsFactory();
        var detail = "useful debug detail";
        var pd = factory.CreateProblemDetails(context, 500, "Err", "https://example.com/probs/err", detail, "/x");

        // In development we expect the factory to preserve the detail for debugging
        Assert.AreEqual(detail, pd.Detail);
        Assert.IsTrue(pd.Extensions.ContainsKey("traceId"));
        Assert.AreEqual("cid-dev-1", pd.Extensions["traceId"]);
    }
}
