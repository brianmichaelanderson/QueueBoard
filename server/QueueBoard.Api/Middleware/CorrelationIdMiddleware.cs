using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace QueueBoard.Api.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        public const string HeaderName = "X-Correlation-ID";

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            // Check incoming header
            string? correlationId = null;
            if (context.Request.Headers.TryGetValue(HeaderName, out var values) && !string.IsNullOrWhiteSpace(values))
            {
                correlationId = values.ToString();
            }

            // Fallback to Activity Id or new Guid
            if (string.IsNullOrWhiteSpace(correlationId))
            {
                correlationId = Activity.Current?.Id ?? Guid.NewGuid().ToString();
            }

            // Expose to context items for downstream usage
            context.Items["CorrelationId"] = correlationId;

            // Set trace identifier so existing code can use it
            try { context.TraceIdentifier = correlationId; } catch { }

            // Ensure response has header early so middleware/tests can observe it
            if (!context.Response.Headers.ContainsKey(HeaderName))
            {
                context.Response.Headers[HeaderName] = correlationId;
            }

            await _next(context).ConfigureAwait(false);
        }
    }
}
