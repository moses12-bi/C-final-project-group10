using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RecommendationRepository : IRecommendationRepository
{
    private readonly AppDbContext _context;

    public RecommendationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Recommendation>> GetByTaskAsync(Guid taskId, CancellationToken ct = default)
    {
        return await _context.Recommendations
            .Where(r => r.TaskItemId == taskId)
            .OrderByDescending(r => r.Score)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Recommendation recommendation, CancellationToken ct = default)
    {
        await _context.Recommendations.AddAsync(recommendation, ct);
        await _context.SaveChangesAsync(ct);
    }
}
