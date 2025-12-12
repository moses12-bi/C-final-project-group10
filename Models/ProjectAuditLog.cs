namespace ProjectM.Models
{
    public class ProjectAuditLog
    {

        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Guid UserId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string ChangeJson { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        //navigation properties

        public Project? Project { get; set; }
        public User? User { get; set; }

    }
}
