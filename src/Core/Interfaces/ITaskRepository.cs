using Core.Enums;
using Core.Models;

namespace Core.Interfaces;

public interface ITaskRepository
{
    Task<TaskItem?> GetAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<TaskItem>> GetByProjectAsync(Guid projectId, CancellationToken ct = default);
    Task AddAsync(TaskItem task, CancellationToken ct = default);
    Task UpdateAsync(TaskItem task, CancellationToken ct = default);
    Task UpdateStatusAsync(Guid taskId, TaskStatus status, CancellationToken ct = default);
}
