namespace ProjectM.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties for permissions & auth
        public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
        public ICollection<Invitation> InvitationsSent { get; set; } = new List<Invitation>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<OtpCode> OtpCodes { get; set; } = new List<OtpCode>();
        
        // Legacy navigation properties (keeping for compatibility)
        public ICollection<Project> ManagedProjects { get; set; } = new List<Project>();
        public ICollection<ProjectTeammember> ProjectTeammembers { get; set; } = new List<ProjectTeammember>();
        public ICollection<TaskAssignment> AssignedTasks { get; set; } = new List<TaskAssignment>();
        public ICollection<PerformanceMetric> PerformanceMetrics { get; set; } = new List<PerformanceMetric>();
        public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
        public ICollection<Notification> Notification { get; set; } = new List<Notification>();
    }
}
