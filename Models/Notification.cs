namespace ProjectM.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public Guid UserId { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReadAt { get; set; }

        // Related entity tracking
        public int? RelatedTaskId { get; set; }
        public int? RelatedProjectId { get; set; }
        public int? RelatedEntityId { get; set; }
        public string? RelatedEntityType { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public ProjectTask? RelatedTask { get; set; }
        public Project? RelatedProject { get; set; }
    }

    // Models/Enums/NotificationType.cs
    public enum NotificationType
    {
        TaskAssigned,
        TaskCompleted,
        TaskOverdue,
        DeadlineReminder,
        ProjectUpdate,
        FeedbackReceived,
        SystemAlert
    }
}
