namespace ProjectM.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Goal { get; set; } = string.Empty;
        public ProjectStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? ActualCompletionDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid ManagerId { get; set; }
        public Guid? TeamLeadId { get; set; }

        //navigation properties

        public User? Manager { get; set; }
        public User? TeamLead { get; set; }
        public ICollection<ProjectTeammember> ProjectTeammembers { get; set; } = new List<ProjectTeammember>();
        public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
        public ICollection<ProjectAuditLog> AuditLogs { get; set; } = new List<ProjectAuditLog>();
        public ICollection<ProjectSummary> Summaries { get; set; } = new List<ProjectSummary>();
    }   
    public enum ProjectStatus
    {
        NotStarted,
        InProgress,
        Completed,
        OnHold,
        Cancelled
    }
}



