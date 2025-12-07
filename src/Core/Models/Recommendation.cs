using Core.Enums;

namespace Core.Models;

public class Recommendation
{
    public Guid Id { get; set; }
    public Guid TaskItemId { get; set; }
    public Guid RecommendedUserId { get; set; }
    public RecommendationType Type { get; set; }
    public double Score { get; set; }
    public string RationaleJson { get; set; } = "{}";
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

    public TaskItem? TaskItem { get; set; }
}
