using System.ComponentModel.DataAnnotations;

namespace QueueBoard.Api.Entities
{
    public class Queue
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public ICollection<AgentQueue> AgentQueues { get; set; } = new List<AgentQueue>();
        
    }
}
