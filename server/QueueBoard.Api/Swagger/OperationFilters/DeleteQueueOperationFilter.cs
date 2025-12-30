using System;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace QueueBoard.Api.Swagger.OperationFilters
{
    public class DeleteQueueOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Only target DELETE /queues/{id}
            var httpMethod = context.ApiDescription.HttpMethod;
            var relativePath = context.ApiDescription.RelativePath?.TrimEnd('/');
            if (!string.Equals(httpMethod, "DELETE", StringComparison.OrdinalIgnoreCase)) return;
            if (relativePath is null) return;
            if (!relativePath.StartsWith("queues/", StringComparison.OrdinalIgnoreCase)) return;

            // 412 Precondition Failed example (application/problem+json)
            if (operation.Responses.TryGetValue("412", out var preconditionResp))
            {
                preconditionResp.Content ??= new System.Collections.Generic.Dictionary<string, OpenApiMediaType>();
                preconditionResp.Content["application/problem+json"] = new OpenApiMediaType
                {
                    Example = new OpenApiObject
                    {
                        ["type"] = new OpenApiString("https://example.com/probs/precondition-failed"),
                        ["title"] = new OpenApiString("Precondition Failed"),
                        ["status"] = new OpenApiInteger(412),
                        ["detail"] = new OpenApiString("The provided ETag does not match the current resource state."),
                        ["instance"] = new OpenApiString("/queues/{id}"),
                        ["traceId"] = new OpenApiString("|trace-id-example|")
                    }
                };
            }

            // 404 NotFound example
            if (operation.Responses.TryGetValue("404", out var notFoundResp))
            {
                notFoundResp.Content ??= new System.Collections.Generic.Dictionary<string, OpenApiMediaType>();
                notFoundResp.Content["application/problem+json"] = new OpenApiMediaType
                {
                    Example = new OpenApiObject
                    {
                        ["type"] = new OpenApiString("https://example.com/probs/not-found"),
                        ["title"] = new OpenApiString("Not Found"),
                        ["status"] = new OpenApiInteger(404),
                        ["detail"] = new OpenApiString("Queue with the specified id was not found."),
                        ["instance"] = new OpenApiString("/queues/{id}"),
                        ["traceId"] = new OpenApiString("|trace-id-example|")
                    }
                };
            }

            // 204 NoContent - Add a short description (no body example needed)
            if (operation.Responses.TryGetValue("204", out var noContentResp))
            {
                noContentResp.Description = "NoContent — queue deleted (idempotent).";
            }
        }
    }
}
