using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace QueueBoard.Api.Services
{
    public class CustomProblemDetailsFactory : ProblemDetailsFactory
    {
        public override ProblemDetails CreateProblemDetails(HttpContext? httpContext, int? statusCode = null, string? title = null, string? type = null, string? detail = null, string? instance = null)
        {
            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Type = type,
                Detail = detail,
                Instance = instance
            };

            EnrichWithDefaults(httpContext, problem);

            return problem;
        }

        public override ValidationProblemDetails CreateValidationProblemDetails(HttpContext? httpContext, ModelStateDictionary modelStateDictionary, int? statusCode = null, string? title = null, string? type = null, string? detail = null, string? instance = null)
        {
            var errors = modelStateDictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception?.Message ?? string.Empty : e.ErrorMessage).ToArray());

            var vpd = new ValidationProblemDetails(errors)
            {
                Status = statusCode ?? StatusCodes.Status400BadRequest,
                Title = title ?? "One or more validation errors occurred.",
                Type = type ?? "https://example.com/probs/validation",
                Detail = detail,
                Instance = instance
            };

            EnrichWithDefaults(httpContext, vpd);

            return vpd;
        }

        private void EnrichWithDefaults(HttpContext? httpContext, ProblemDetails pd)
        {
            if (httpContext != null)
            {
                var traceId = httpContext.Items.ContainsKey("CorrelationId") ? httpContext.Items["CorrelationId"]?.ToString() : httpContext.TraceIdentifier;
                if (!string.IsNullOrEmpty(traceId)) pd.Extensions["traceId"] = traceId!;
            }

            pd.Extensions["timestamp"] = DateTime.UtcNow.ToString("o");
        }
    }
}
