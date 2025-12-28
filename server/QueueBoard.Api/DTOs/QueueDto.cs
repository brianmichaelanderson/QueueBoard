using System;

namespace QueueBoard.Api.DTOs
{
    /// <summary>
    /// Data transfer object for a queue.
    /// </summary>
    public sealed record QueueDto(
        Guid Id,
        string Name,
        string? Description,
        bool IsActive,
        DateTimeOffset CreatedAt
    );
}
