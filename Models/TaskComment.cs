namespace ProjectM.Models
{
    public class TaskComment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ProjectTask? Task { get; set; }
        public User? User { get; set; }
    }
}
