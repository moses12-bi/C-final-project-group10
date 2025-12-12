using System.Text.Json.Serialization;

namespace ProjectM.Models
{
    public class Invitation
    {
        public Guid InvitationId { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string PermissionsJson { get; set; } = string.Empty;
        public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
        public DateTime ExpiresAt { get; set; }
        public Guid InvitedByUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [JsonIgnore]
        public User InvitedByUser { get; set; } = null!;
    }

    public enum InvitationStatus
    {
        Pending,
        Used,
        Expired
    }
}
