using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QueueBoard.Api.Tests.Unit;

[TestClass]
public class ExceptionHandlingMiddlewareTests
{
    [TestMethod]
    public async Task UnhandledException_ReturnsProblemDetails500()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/test/unhandled";
        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        RequestDelegate next = _ => throw new Exception("boom");

        var logger = NullLoggerFactory.Instance.CreateLogger("tests");

        var middleware = new ExceptionHandlingMiddleware(next, logger);

        await middleware.InvokeAsync(context);

        responseStream.Seek(0, SeekOrigin.Begin);
        var text = new StreamReader(responseStream).ReadToEnd();

        Assert.AreEqual("application/problem+json", context.Response.ContentType);
        Assert.AreEqual((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);

        using var doc = JsonDocument.Parse(text);
        Assert.IsTrue(doc.RootElement.TryGetProperty("title", out var _));
    }

    [TestMethod]
    public async Task KeyNotFoundException_Returns404()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/test/notfound";
        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        RequestDelegate next = _ => throw new System.Collections.Generic.KeyNotFoundException("missing");

        var logger = NullLoggerFactory.Instance.CreateLogger("tests");
        var middleware = new ExceptionHandlingMiddleware(next, logger);

        await middleware.InvokeAsync(context);

        Assert.AreEqual("application/problem+json", context.Response.ContentType);
        Assert.AreEqual((int)HttpStatusCode.NotFound, context.Response.StatusCode);
    }

    [TestMethod]
    public async Task ValidationException_Returns400()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/test/validation";
        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        RequestDelegate next = _ => throw new System.ComponentModel.DataAnnotations.ValidationException("invalid");

        var logger = NullLoggerFactory.Instance.CreateLogger("tests");
        var middleware = new ExceptionHandlingMiddleware(next, logger);

        await middleware.InvokeAsync(context);

        Assert.AreEqual("application/problem+json", context.Response.ContentType);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
    }
}
