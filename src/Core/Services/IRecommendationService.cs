using Core.Enums;
using Core.Models;
using Core.DTOs;

namespace Core.Services;

public interface IRecommendationService
{
    Task<IEnumerable<EmployeeRecommendationDto>> GetBestEmployeesForTaskAsync(Guid taskId, CancellationToken ct = default);
    Task<IEnumerable<TeamMemberRecommendationDto>> GetBestTeamMembersForProjectAsync(Guid projectId, CancellationToken ct = default);
    Task<DeadlineRecommendationDto> GetEstimatedDeadlineAsync(Guid taskId, CancellationToken ct = default);
    Task<RiskAssessmentDto> AssessProjectRiskAsync(Guid projectId, CancellationToken ct = default);
    Task<WorkloadImpactDto> AssessWorkloadImpactAsync(Guid employeeId, Guid taskId, CancellationToken ct = default);
    Task<double> CalculateTaskDifficultyAsync(CreateTaskRequest taskRequest, CancellationToken ct = default);
}

public record EmployeeRecommendationDto(
    Guid EmployeeId,
    string EmployeeName,
    double Score,
    string Rationale,
    double SkillsMatch,
    double WorkloadAvailability,
    double PastPerformance,
    double CurrentWorkload
);

public record TeamMemberRecommendationDto(
    Guid EmployeeId,
    string EmployeeName,
    double Score,
    string Rationale,
    string[] MatchingSkills
);

public record DeadlineRecommendationDto(
    DateTime EstimatedCompletionDate,
    int EstimatedDays,
    double Confidence,
    string[] RiskFactors
);

public record RiskAssessmentDto(
    double RiskScore,
    string RiskLevel,
    string[] RiskFactors,
    string[] MitigationStrategies
);

public record WorkloadImpactDto(
    double CurrentWorkload,
    double ProjectedWorkload,
    double ImpactPercentage,
    bool IsOverloaded,
    string Recommendation
);
