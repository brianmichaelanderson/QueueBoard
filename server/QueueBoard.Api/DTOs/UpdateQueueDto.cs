using System.ComponentModel.DataAnnotations;

namespace QueueBoard.Api.DTOs
{
    /// <summary>
    /// DTO used for updating an existing queue. Includes the RowVersion (base64) for optimistic concurrency.
    /// </summary>
    public sealed record UpdateQueueDto(
        [param: Required]
        string Name,
        string? Description,
        bool IsActive,
        /// <summary>
        /// Base64-encoded RowVersion token from the server to support optimistic concurrency.
        /// Optional for now; required behavior will be enforced when controller/service updated.
        /// </summary>
        string? RowVersion
    );
}
