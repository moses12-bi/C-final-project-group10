using Core.Enums;

namespace Core.DTOs;

public record CreateTaskRequest(
    Guid ProjectId,
    Guid CreatedById,
    Guid? AssignedToId,
    string Title,
    string Description,
    TaskPriority Priority,
    double EstimatedEffort,
    DateTime? DueDate,
    int DifficultyScore,
    Guid? DependencyTaskId
);
