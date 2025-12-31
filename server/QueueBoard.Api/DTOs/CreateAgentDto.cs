using System.ComponentModel.DataAnnotations;

namespace QueueBoard.Api.DTOs
{
    /// <summary>
    /// DTO used to create a new agent.
    /// </summary>
    /// <param name="FirstName">Agent first name.</param>
    /// <param name="LastName">Agent last name.</param>
    /// <param name="Email">Agent email address.</param>
    /// <param name="IsActive">Whether the agent is active.</param>
    public sealed record CreateAgentDto(
        [param: Required]
        [property: StringLength(100)]
        string FirstName,

        [param: Required]
        [property: StringLength(100)]
        string LastName,

        [param: Required]
        [property: StringLength(256)]
        [property: EmailAddress]
        string Email,

        bool IsActive
    );
}
