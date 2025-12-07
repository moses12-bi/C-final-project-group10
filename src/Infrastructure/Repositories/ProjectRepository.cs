using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _context;

    public ProjectRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Project?> GetAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Projects
            .Include(p => p.Manager)
            .Include(p => p.Members)
            .ThenInclude(pm => pm.User)
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<IReadOnlyList<Project>> GetForUserAsync(Guid userId, CancellationToken ct = default)
    {
        return await _context.Projects
            .Where(p => p.ManagerId == userId || p.Members.Any(m => m.UserId == userId))
            .Include(p => p.Manager)
            .Include(p => p.Members)
            .ThenInclude(pm => pm.User)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Project project, CancellationToken ct = default)
    {
        await _context.Projects.AddAsync(project, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Project project, CancellationToken ct = default)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync(ct);
    }
}
