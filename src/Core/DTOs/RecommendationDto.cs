using Core.Enums;

namespace Core.DTOs;

public record RecommendationDto(
    Guid Id,
    Guid TaskItemId,
    Guid RecommendedUserId,
    RecommendationType Type,
    double Score,
    string RationaleJson,
    DateTime GeneratedAt
);
