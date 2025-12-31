using System.ComponentModel.DataAnnotations;

namespace QueueBoard.Api.DTOs
{
    /// <summary>
    /// DTO used to create a new agent.
    /// </summary>
    /// <example>
    /// { "firstName": "Alice", "lastName": "Anderson", "email": "alice.anderson@example.com", "isActive": true }
    /// </example>
    /// <param name="FirstName">Agent first name.</param>
    /// <param name="LastName">Agent last name.</param>
    /// <param name="Email">Agent email address.</param>
    /// <param name="IsActive">Whether the agent is active.</param>
    public sealed record CreateAgentDto(
        [param: Required]
        [param: StringLength(100)]
        string FirstName,

        [param: Required]
        [param: StringLength(100)]
        string LastName,

        [param: Required]
        [param: StringLength(256)]
        [param: EmailAddress]
        string Email,

        bool IsActive
    );
}
