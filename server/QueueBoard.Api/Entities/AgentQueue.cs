namespace QueueBoard.Api.Entities
{
    public class AgentQueue
    {
        public Guid AgentId { get; set; }
        public Agent Agent { get; set; } = null!;

        public Guid QueueId { get; set; }
        public Queue Queue { get; set; } = null!;

        public bool IsPrimary { get; set; }

        public int SkillLevel { get; set; }

        public DateTimeOffset AssignedAt { get; set; }
    }
}
