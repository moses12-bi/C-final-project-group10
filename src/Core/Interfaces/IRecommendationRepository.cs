using Core.Models;

namespace Core.Interfaces;

public interface IRecommendationRepository
{
    Task<IReadOnlyList<Recommendation>> GetByTaskAsync(Guid taskId, CancellationToken ct = default);
    Task AddAsync(Recommendation recommendation, CancellationToken ct = default);
}
