using System.ComponentModel.DataAnnotations;

namespace QueueBoard.Api.Entities
{
    public class Agent
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public ICollection<AgentQueue> AgentQueues { get; set; } = new List<AgentQueue>();
    }
}
