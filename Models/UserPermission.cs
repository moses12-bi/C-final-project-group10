namespace ProjectM.Models
{
    public class UserPermission
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string PermissionKey { get; set; } = string.Empty;
        public bool IsGranted { get; set; } = true;

        // Navigation properties
        public User User { get; set; } = null!;
    }
}
