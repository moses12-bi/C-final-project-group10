namespace ProjectM.Models
{
    public class OtpCode
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsUsed { get; set; } = false;

        // Navigation properties
        public User User { get; set; } = null!;
    }
}
