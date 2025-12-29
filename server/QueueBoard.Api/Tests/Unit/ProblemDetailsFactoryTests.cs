using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QueueBoard.Api.Tests.Unit;

[TestClass]
public class ProblemDetailsFactoryTests
{
    [TestMethod]
    public void CreateProblemDetails_IncludesTraceIdAndTimestamp()
    {
        var context = new DefaultHttpContext();
        context.Items["CorrelationId"] = "cid-123";

        var factory = new QueueBoard.Api.Services.CustomProblemDetailsFactory();
        var pd = factory.CreateProblemDetails(context, 500, "err", "type", "detail", "/p");

        Assert.AreEqual(500, pd.Status);
        Assert.IsTrue(pd.Extensions.ContainsKey("traceId"));
        Assert.AreEqual("cid-123", pd.Extensions["traceId"]);
        Assert.IsTrue(pd.Extensions.ContainsKey("timestamp"));
    }

    [TestMethod]
    public void CreateValidationProblemDetails_PopulatesErrorsAndTraceId()
    {
        var context = new DefaultHttpContext();
        context.Items["CorrelationId"] = "cid-456";

        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Name", "Required");

        var factory = new QueueBoard.Api.Services.CustomProblemDetailsFactory();
        var vpd = factory.CreateValidationProblemDetails(context, modelState, 400, "Validation Error");

        Assert.IsNotNull(vpd);
        Assert.AreEqual(400, vpd.Status);
        Assert.IsTrue(vpd.Errors.ContainsKey("Name"));
        Assert.AreEqual("Required", vpd.Errors["Name"][0]);
        Assert.IsTrue(vpd.Extensions.ContainsKey("traceId"));
        Assert.AreEqual("cid-456", vpd.Extensions["traceId"]);
    }
}
