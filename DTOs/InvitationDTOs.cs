using System.ComponentModel.DataAnnotations;
using ProjectM.Models;

namespace ProjectM.DTOs
{
    public class CreateInvitationRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;

        [Required]
        public string Department { get; set; } = string.Empty;

        [Required]
        public Dictionary<string, bool> Permissions { get; set; } = new Dictionary<string, bool>();
    }

    public class InvitationResponse
    {
        public Guid InvitationId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public Dictionary<string, bool> Permissions { get; set; } = new Dictionary<string, bool>();
        public InvitationStatus Status { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string InvitedByUserName { get; set; } = string.Empty;
    }

    public class CompleteRegistrationRequest
    {
        [Required]
        public Guid Token { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        public Dictionary<string, bool> Permissions { get; set; } = new Dictionary<string, bool>();
    }

    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public Dictionary<string, bool> Permissions { get; set; } = new Dictionary<string, bool>();
    }
}
