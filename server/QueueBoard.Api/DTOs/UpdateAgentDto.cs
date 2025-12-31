using System.ComponentModel.DataAnnotations;

namespace QueueBoard.Api.DTOs
{
    /// <summary>
    /// DTO used for updating an existing agent. Includes the RowVersion (base64) for optimistic concurrency.
    /// </summary>
    /// <example>
    /// { "firstName": "Alice", "lastName": "Anderson", "email": "alice.anderson@example.com", "isActive": true, "rowVersion": "<base64-token>" }
    /// </example>
    /// <param name="FirstName">Agent first name.</param>
    /// <param name="LastName">Agent last name.</param>
    /// <param name="Email">Agent email address.</param>
    /// <param name="IsActive">Whether the agent is active.</param>
    /// <param name="RowVersion">Base64-encoded RowVersion token from the server to support optimistic concurrency. Optional for now.</param>
    public sealed record UpdateAgentDto(
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

        bool IsActive,

        string? RowVersion
    );
}
