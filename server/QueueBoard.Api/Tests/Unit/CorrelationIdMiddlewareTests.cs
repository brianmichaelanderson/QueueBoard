using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QueueBoard.Api.Tests.Unit;

[TestClass]
public class CorrelationIdMiddlewareTests
{
    private const string HeaderName = "X-Correlation-ID";

    [TestMethod]
    public async Task AddsCorrelationId_WhenMissing()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/test/noheader";
        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        RequestDelegate next = ctx =>
        {
            Assert.IsTrue(ctx.Response.Headers.ContainsKey(HeaderName));
            var id = ctx.Response.Headers[HeaderName].ToString();
            Assert.IsFalse(string.IsNullOrWhiteSpace(id));
            ctx.Items["captured"] = id;
            return Task.CompletedTask;
        };

        var middleware = new CorrelationIdMiddleware(next);

        await middleware.InvokeAsync(context);

        Assert.IsTrue(context.Items.ContainsKey("captured"));
    }

    [TestMethod]
    public async Task PreservesIncomingCorrelationId()
    {
        var context = new DefaultHttpContext();
        var incoming = "incoming-123";
        context.Request.Headers[HeaderName] = incoming;
        context.Request.Path = "/test/withheader";
        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        RequestDelegate next = ctx =>
        {
            Assert.IsTrue(ctx.Response.Headers.ContainsKey(HeaderName));
            Assert.AreEqual(incoming, ctx.Response.Headers[HeaderName].ToString());
            ctx.Items["captured"] = ctx.Response.Headers[HeaderName].ToString();
            return Task.CompletedTask;
        };

        var middleware = new CorrelationIdMiddleware(next);

        await middleware.InvokeAsync(context);

        Assert.AreEqual(incoming, context.Items["captured"]);
    }

    [TestMethod]
    public async Task SetsCorrelationIdInContextItems()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/test/items";
        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        RequestDelegate next = ctx =>
        {
            Assert.IsTrue(ctx.Items.ContainsKey("CorrelationId"));
            var id = ctx.Items["CorrelationId"] as string;
            Assert.IsFalse(string.IsNullOrEmpty(id));
            return Task.CompletedTask;
        };

        var middleware = new CorrelationIdMiddleware(next);

        await middleware.InvokeAsync(context);
    }
}
