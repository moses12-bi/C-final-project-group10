using Core.Enums;

namespace Core.Models;

public class TaskItem
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? AssignedToId { get; set; }
    public Guid CreatedById { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskStatus Status { get; set; } = TaskStatus.ToDo;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public double EstimatedEffort { get; set; }
    public DateTime? DueDate { get; set; }
    public int DifficultyScore { get; set; }
    public Guid? DependencyTaskId { get; set; }

    public Project? Project { get; set; }
    public ICollection<TaskUpdate> Updates { get; set; } = new List<TaskUpdate>();
    public ICollection<Recommendation> Recommendations { get; set; } = new List<Recommendation>();
}
