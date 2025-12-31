using System.ComponentModel.DataAnnotations;

namespace QueueBoard.Api.DTOs
{
    /// <summary>
    /// DTO used to create a new queue.
    /// </summary>
    /// <example>
    /// { "name": "Support", "description": "Customer support queue", "isActive": true }
    /// </example>
    public sealed record CreateQueueDto(
        [param: Required]
        string Name,
        string? Description,
        bool IsActive
    );
}
