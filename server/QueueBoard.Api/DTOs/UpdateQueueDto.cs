using System.ComponentModel.DataAnnotations;

namespace QueueBoard.Api.DTOs
{
    /// <summary>
    /// DTO used for updating an existing queue. Includes the RowVersion (base64) for optimistic concurrency.
    /// </summary>
    /// <example>
    /// { "name": "Support", "description": "Customer support queue", "isActive": true, "rowVersion": "<base64-token>" }
    /// </example>
    /// <param name="Name">The queue name.</param>
    /// <param name="Description">Optional human-readable description for the queue.</param>
    /// <param name="IsActive">Whether the queue is active.</param>
    /// <param name="RowVersion">Base64-encoded RowVersion token from the server to support optimistic concurrency. Optional for now.</param>
    public sealed record UpdateQueueDto(
        [param: Required]
        string Name,
        string? Description,
        bool IsActive,
        string? RowVersion
    );
}
