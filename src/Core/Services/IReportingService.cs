using Core.DTOs;

namespace Core.Services;

public interface IReportingService
{
    Task<ProjectMetricsDto> GetProjectMetricsAsync(Guid projectId, CancellationToken ct = default);
    Task<TeamPerformanceDto> GetTeamPerformanceAsync(Guid projectId, CancellationToken ct = default);
    Task<EmployeeProductivityDto> GetEmployeeProductivityAsync(Guid employeeId, CancellationToken ct = default);
    Task<BottleneckAnalysisDto> GetBottleneckAnalysisAsync(Guid projectId, CancellationToken ct = default);
    Task<CompletionMetricsDto> GetCompletionMetricsAsync(Guid projectId, CancellationToken ct = default);
    Task<IEnumerable<RiskAlertDto>> GetRiskAlertsAsync(Guid projectId, CancellationToken ct = default);
    Task<WorkloadReportDto> GetWorkloadReportAsync(Guid projectId, CancellationToken ct = default);
}

public record ProjectMetricsDto(
    Guid ProjectId,
    string ProjectName,
    int TotalTasks,
    int CompletedTasks,
    int InProgressTasks,
    int OverdueTasks,
    double CompletionPercentage,
    double AverageTaskDuration,
    DateTime? ProjectedCompletionDate,
    double BudgetUtilization,
    string Status
);

public record TeamPerformanceDto(
    Guid ProjectId,
    IEnumerable<TeamMemberPerformanceDto> Members,
    double AverageProductivity,
    double AverageQualityScore,
    int TotalTasksCompleted,
    double AverageCompletionTime
);

public record TeamMemberPerformanceDto(
    Guid EmployeeId,
    string EmployeeName,
    int TasksCompleted,
    int TasksInProgress,
    double ProductivityScore,
    double QualityScore,
    double AverageCompletionTime,
    double CurrentWorkload,
    double UtilizationRate
);

public record EmployeeProductivityDto(
    Guid EmployeeId,
    string EmployeeName,
    int TotalTasks,
    int CompletedTasks,
    double CompletionRate,
    double AverageTaskDuration,
    double PerformanceScore,
    IEnumerable<TaskPerformanceDto> RecentTasks
);

public record TaskPerformanceDto(
    Guid TaskId,
    string TaskTitle,
    DateTime CompletedAt,
    double Duration,
    double QualityScore
);

public record BottleneckAnalysisDto(
    Guid ProjectId,
    IEnumerable<BottleneckDto> Bottlenecks,
    string PrimaryBottleneck,
    string[] Recommendations
);

public record BottleneckDto(
    string Type,
    string Description,
    int AffectedTasks,
    double ImpactScore,
    string Severity
);

public record CompletionMetricsDto(
    Guid ProjectId,
    int TasksCompletedOnTime,
    int TasksCompletedLate,
    double OnTimeCompletionRate,
    double AverageDelay,
    IEnumerable<MonthlyCompletionDto> MonthlyTrends
);

public record MonthlyCompletionDto(
    int Month,
    int Year,
    int TasksCompleted,
    double CompletionRate
);

public record RiskAlertDto(
    Guid ProjectId,
    string RiskType,
    string Description,
    string Severity,
    int AffectedTasks,
    DateTime DetectedAt,
    string[] MitigationSteps
);

public record WorkloadReportDto(
    Guid ProjectId,
    IEnumerable<EmployeeWorkloadDto> EmployeeWorkloads,
    double TotalWorkload,
    double AverageWorkload,
    int OverloadedEmployees,
    int UnderutilizedEmployees
);

public record EmployeeWorkloadDto(
    Guid EmployeeId,
    string EmployeeName,
    double CurrentWorkload,
    double RecommendedWorkload,
    double UtilizationRate,
    string WorkloadStatus,
    string[] Recommendations
);
