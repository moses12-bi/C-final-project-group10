namespace ProjectM.Models
{
    public class AccessRequest
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public AccessRequestStatus Status { get; set; } = AccessRequestStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }
        public Guid? ReviewedByUserId { get; set; }
        public string? ReviewNotes { get; set; }

        // Navigation properties
        public User? ReviewedByUser { get; set; }
    }

    public enum AccessRequestStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
