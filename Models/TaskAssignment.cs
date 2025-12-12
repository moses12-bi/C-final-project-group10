namespace ProjectM.Models
{
    public class TaskAssignment
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public Guid UserId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UnassignedAt { get; set; }
        public bool IsPrimaryAssignee { get; set; } = true;

        // Navigation properties
        public ProjectTask? Task { get; set; }
        public User? User { get; set; }
    }
}
