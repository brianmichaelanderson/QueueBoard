using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace QueueBoard.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex).ConfigureAwait(false);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            int status;
            object body;

            switch (ex)
            {
                case ValidationException vex:
                    status = (int)HttpStatusCode.BadRequest;
                    body = new
                    {
                        type = "https://example.com/probs/validation",
                        title = "One or more validation errors occurred.",
                        status,
                        errors = new Dictionary<string, string[]>
                        {
                            { "Validation", new[] { vex.Message } }
                        },
                        instance = context.Request.Path.Value,
                        traceId = context.Items.ContainsKey("CorrelationId") ? context.Items["CorrelationId"] : context.TraceIdentifier,
                        timestamp = DateTime.UtcNow.ToString("o")
                    };
                    _logger.LogWarning(ex, "Validation error processing request {Path}", context.Request.Path);
                    break;

                case KeyNotFoundException knf:
                    status = (int)HttpStatusCode.NotFound;
                    body = new
                    {
                        type = "https://example.com/probs/not-found",
                        title = "Resource not found.",
                        status,
                        detail = knf.Message,
                        instance = context.Request.Path.Value,
                        traceId = context.Items.ContainsKey("CorrelationId") ? context.Items["CorrelationId"] : context.TraceIdentifier,
                        timestamp = DateTime.UtcNow.ToString("o")
                    };
                    _logger.LogWarning(ex, "Not found while processing request {Path}", context.Request.Path);
                    break;

                default:
                    status = (int)HttpStatusCode.InternalServerError;
                    body = new
                    {
                        type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                        title = "An unexpected error occurred.",
                        status,
                        detail = ex.Message,
                        instance = context.Request.Path.Value,
                        traceId = context.Items.ContainsKey("CorrelationId") ? context.Items["CorrelationId"] : context.TraceIdentifier,
                        timestamp = DateTime.UtcNow.ToString("o")
                    };
                    _logger.LogError(ex, "Unhandled exception while processing request {Path}", context.Request.Path);
                    break;
            }

            var json = JsonSerializer.Serialize(body);
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = status;
            return context.Response.WriteAsync(json);
        }
    }
}
