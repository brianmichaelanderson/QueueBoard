using System;
using System.ComponentModel.DataAnnotations;

namespace QueueBoard.Api.DTOs
{
    /// <summary>
    /// Data transfer object for an agent.
    /// </summary>
    public sealed record AgentDto(
        Guid Id,
        [property: Required]
        [property: StringLength(100)]
        string FirstName,
        [property: Required]
        [property: StringLength(100)]
        string LastName,
        [property: Required]
        [property: StringLength(256)]
        [property: EmailAddress]
        string Email,
        bool IsActive,
        DateTimeOffset CreatedAt
    );
}
