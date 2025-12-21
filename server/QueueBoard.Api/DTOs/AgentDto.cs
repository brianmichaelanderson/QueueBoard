using System;

namespace QueueBoard.Api.DTOs
{
    public sealed record AgentDto(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        bool IsActive,
        DateTimeOffset CreatedAt
    );
}
