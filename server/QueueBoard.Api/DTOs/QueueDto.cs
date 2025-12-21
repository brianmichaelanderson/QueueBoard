using System;

namespace QueueBoard.Api.DTOs
{
    public sealed record QueueDto(
        Guid Id,
        string Name,
        string? Description,
        bool IsActive,
        DateTimeOffset CreatedAt
    );
}
