using System;
using System.ComponentModel.DataAnnotations;

namespace QueueBoard.Api.DTOs
{
    /// <summary>
    /// Data transfer object for a queue.
    /// </summary>
    public sealed record QueueDto(
        Guid Id,
        [property: Required]
        string Name,
        string? Description,
        bool IsActive,
        DateTimeOffset CreatedAt,
        // Base64-encoded RowVersion token for optimistic concurrency.
        string? RowVersion
    );
}
