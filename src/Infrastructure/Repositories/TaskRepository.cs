using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TaskItem?> GetAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.AssignedTo)
            .Include(t => t.CreatedBy)
            .Include(t => t.Updates)
            .Include(t => t.Recommendations)
            .FirstOrDefaultAsync(t => t.Id == id, ct);
    }

    public async Task<IReadOnlyList<TaskItem>> GetByProjectAsync(Guid projectId, CancellationToken ct = default)
    {
        return await _context.Tasks
            .Where(t => t.ProjectId == projectId)
            .Include(t => t.AssignedTo)
            .Include(t => t.CreatedBy)
            .OrderBy(t => t.DueDate)
            .ToListAsync(ct);
    }

    public async Task AddAsync(TaskItem task, CancellationToken ct = default)
    {
        await _context.Tasks.AddAsync(task, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(TaskItem task, CancellationToken ct = default)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateStatusAsync(Guid taskId, TaskStatus status, CancellationToken ct = default)
    {
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId, ct);
        if (task != null)
        {
            task.Status = status;
            await _context.SaveChangesAsync(ct);
        }
    }
}
