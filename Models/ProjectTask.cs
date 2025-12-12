namespace ProjectM.Models
{
    public class ProjectTask
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskPriority Priority { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime StartDate { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime? CompletedAt { get; set; }
        public decimal EstimatedHours { get; set; }
        public decimal? ActualHours { get; set; }
        public int ProjectId { get; set; }
        public int? ParentTaskId { get; set; }
        //navigation properties
        public Project? Project { get; set; }
        public ProjectTask? ParentTask { get; set; }
        public ICollection<ProjectTask> SubTasks { get; set; } = new List<ProjectTask>();
        public ICollection<TaskAssignment> Assignments { get; set; } = new List<TaskAssignment>();
        public ICollection<TaskDependency> Dependencies { get; set; } = new List<TaskDependency>();
        public ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
        public ICollection<TaskAttachment> Attachments { get; set; } = new List<TaskAttachment>();
        public ICollection<TaskAuditLog> AuditLogs { get; set; } = new List<TaskAuditLog>();

    }
    public enum TaskStatus
    {
        ToDo,
        InProgress,
        Review,
        Done,
        Block }
    public enum TaskPriority
    {
        Low,
        Medium,
        High,
        Critical
    }



}
