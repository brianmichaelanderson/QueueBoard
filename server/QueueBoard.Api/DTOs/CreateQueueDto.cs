using System.ComponentModel.DataAnnotations;

namespace QueueBoard.Api.DTOs
{
    public sealed record CreateQueueDto(
        [param: Required]
        string Name,
        string? Description,
        bool IsActive
    );
}
