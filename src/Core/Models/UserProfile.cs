using Core.Enums;

namespace Core.Models;

public class UserProfile
{
    public Guid Id { get; set; }
    public Guid IdentityUserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string SkillsJson { get; set; } = "[]";
    public double CurrentWorkload { get; set; }
    public double PerformanceScore { get; set; }

    public ICollection<ProjectMember> ProjectMemberships { get; set; } = new List<ProjectMember>();
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public ICollection<TaskUpdate> TaskUpdates { get; set; } = new List<TaskUpdate>();
}
