using Core.Models;

namespace Core.Interfaces;

public interface ITaskUpdateRepository
{
    Task AddAsync(TaskUpdate update, CancellationToken ct = default);
    Task<IReadOnlyList<TaskUpdate>> GetByTaskAsync(Guid taskId, CancellationToken ct = default);
}
