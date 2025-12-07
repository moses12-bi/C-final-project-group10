using Core.Enums;
using Core.Models;
using Core.DTOs;

namespace Core.Services;

public interface ITaskManagementService
{
    Task<TaskItem> CreateTaskAsync(CreateTaskRequest request, CancellationToken ct = default);
    Task<TaskItem> UpdateTaskStatusAsync(Guid taskId, TaskStatus newStatus, Guid updatedById, string? comment = null, CancellationToken ct = default);
    Task<TaskItem> AssignTaskAsync(Guid taskId, Guid assignedToId, CancellationToken ct = default);
    Task<IEnumerable<TaskItem>> GetTasksByProjectAsync(Guid projectId, CancellationToken ct = default);
    Task<IEnumerable<TaskItem>> GetTasksByEmployeeAsync(Guid employeeId, CancellationToken ct = default);
    Task<TaskUpdate> AddTaskUpdateAsync(Guid taskId, Guid updatedById, string comment, double effortLogged, CancellationToken ct = default);
    Task<IEnumerable<TaskUpdate>> GetTaskUpdatesAsync(Guid taskId, CancellationToken ct = default);
    Task<bool> CanUserAccessTaskAsync(Guid userId, Guid taskId, CancellationToken ct = default);
    Task<IEnumerable<TaskItem>> GetOverdueTasksAsync(CancellationToken ct = default);
    Task<IEnumerable<TaskItem>> GetCriticalTasksAsync(CancellationToken ct = default);
}

public record TaskCreationResult(
    TaskItem Task,
    IEnumerable<EmployeeRecommendationDto> Recommendations,
    DeadlineRecommendationDto DeadlineEstimate
);
