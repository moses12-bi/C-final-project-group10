using System.ComponentModel.DataAnnotations;

namespace ProjectM.DTOs
{
    public class InviteUserRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;

        [Required]
        public string Department { get; set; } = string.Empty;

        public Guid InvitedByUserId { get; set; }

        public Dictionary<string, bool> Permissions { get; set; } = new Dictionary<string, bool>();
    }

    public class ChangeRoleRequest
    {
        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
