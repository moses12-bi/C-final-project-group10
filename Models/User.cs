using Microsoft.AspNetCore.Identity;

namespace ProjectM.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; } // Deprecated after permission migration
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public ICollection<Project> ManagedProjects { get; set; } = new List<Project>();
        public ICollection<ProjectTeammember> ProjectTeammembers { get; set; } = new List<ProjectTeammember>();
        public ICollection<TaskAssignment> AssignedTasks { get; set; } = new List<TaskAssignment>();
        public ICollection<PerformanceMetric> PerformanceMetrics { get; set; } = new List<PerformanceMetric>();
        public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
        public ICollection<Notification> Notification { get; set; } = new List<Notification>();

        // New navigation for permissions & auth
        public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<OtpCode> OtpCodes { get; set; } = new List<OtpCode>();
        public ICollection<Invitation> InvitationsSent { get; set; } = new List<Invitation>();
    }

    public enum UserRole 
    {
        Manager,
        TeamLeader,
        Employee
    }
}
