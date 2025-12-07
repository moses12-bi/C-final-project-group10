using Core.Enums;

namespace Core.DTOs;

public record CreateProjectRequest(
    string Name,
    string Description,
    DateTime StartDate,
    DateTime? EndDate,
    decimal RiskLevel,
    Guid ManagerId,
    IEnumerable<Guid> TeamMemberIds
);
