namespace ProjectM.Models
{
    public class TaskAuditLog
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public Guid UserID { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string ChangeJson { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        //navigation properties

        public  ProjectTask? Task { get; set; }
        public User? User { get; set; }

    }
}
