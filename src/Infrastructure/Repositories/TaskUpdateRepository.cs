using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TaskUpdateRepository : ITaskUpdateRepository
{
    private readonly AppDbContext _context;

    public TaskUpdateRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TaskUpdate update, CancellationToken ct = default)
    {
        await _context.TaskUpdates.AddAsync(update, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<TaskUpdate>> GetByTaskAsync(Guid taskId, CancellationToken ct = default)
    {
        return await _context.TaskUpdates
            .Where(tu => tu.TaskItemId == taskId)
            .Include(tu => tu.UpdatedBy)
            .OrderByDescending(tu => tu.CreatedAt)
            .ToListAsync(ct);
    }
}
