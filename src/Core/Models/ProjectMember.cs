using Core.Enums;

namespace Core.Models;

public class ProjectMember
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public UserRole Role { get; set; }
    public int AllocationPercent { get; set; }

    public Project? Project { get; set; }
    public UserProfile? User { get; set; }
}
