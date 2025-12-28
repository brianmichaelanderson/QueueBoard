using System;

namespace QueueBoard.Api.DTOs
{
    /// <summary>
    /// Data transfer object for an agent.
    /// </summary>
    public sealed record AgentDto(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        bool IsActive,
        DateTimeOffset CreatedAt
    );
}
