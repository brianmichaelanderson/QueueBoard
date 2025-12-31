using System.ComponentModel.DataAnnotations;

namespace QueueBoard.Api.DTOs
{
    /// <summary>
    /// DTO used for updating an existing agent. Includes the RowVersion (base64) for optimistic concurrency.
    /// </summary>
    /// <param name="FirstName">Agent first name.</param>
    /// <param name="LastName">Agent last name.</param>
    /// <param name="Email">Agent email address.</param>
    /// <param name="IsActive">Whether the agent is active.</param>
    /// <param name="RowVersion">Base64-encoded RowVersion token from the server to support optimistic concurrency. Optional for now.</param>
    public sealed record UpdateAgentDto(
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

        bool IsActive,

        string? RowVersion
    );
}
