using Core.Enums;

namespace Core.Models;

public class TaskUpdate
{
    public Guid Id { get; set; }
    public Guid TaskItemId { get; set; }
    public Guid UpdatedById { get; set; }
    public string? Comment { get; set; }
    public string? AttachmentUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public double EffortLogged { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.ToDo;

    public TaskItem? TaskItem { get; set; }
    public UserProfile? UpdatedBy { get; set; }
}
