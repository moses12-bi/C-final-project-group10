using Core.Services;
using Core.DTOs;
using Core.Interfaces;
using Core.Models;
using Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class ReportingService : IReportingService
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly ITaskUpdateRepository _taskUpdateRepository;
    private readonly AppDbContext _context;

    public ReportingService(
        IProjectRepository projectRepository,
        ITaskRepository taskRepository,
        ITaskUpdateRepository taskUpdateRepository,
        AppDbContext context)
    {
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
        _taskUpdateRepository = taskUpdateRepository;
        _context = context;
    }

    public async Task<ProjectMetricsDto> GetProjectMetricsAsync(Guid projectId, CancellationToken ct = default)
    {
        var project = await _projectRepository.GetAsync(projectId, ct);
        if (project == null) throw new ArgumentException("Project not found");

        var tasks = await _taskRepository.GetByProjectAsync(projectId, ct);
        var completedTasks = tasks.Where(t => t.Status == TaskStatus.Done).ToList();
        var inProgressTasks = tasks.Where(t => t.Status == TaskStatus.InProgress).ToList();
        var overdueTasks = tasks.Where(t => t.DueDate.HasValue && t.DueDate.Value < DateTime.UtcNow && t.Status != TaskStatus.Done).ToList();

        var completionPercentage = tasks.Count > 0 ? (double)completedTasks.Count / tasks.Count * 100 : 0;
        var averageTaskDuration = completedTasks.Any() ? CalculateAverageTaskDuration(completedTasks) : 0;
        var projectedCompletionDate = CalculateProjectedCompletionDate(project, tasks, completedTasks.Count);

        return new ProjectMetricsDto(
            projectId,
            project.Name,
            tasks.Count,
            completedTasks.Count,
            inProgressTasks.Count,
            overdueTasks.Count,
            completionPercentage,
            averageTaskDuration,
            projectedCompletionDate,
            0, // Budget utilization would be calculated based on financial data
            project.Status.ToString()
        );
    }

    public async Task<TeamPerformanceDto> GetTeamPerformanceAsync(Guid projectId, CancellationToken ct = default)
    {
        var project = await _projectRepository.GetAsync(projectId, ct);
        if (project == null) throw new ArgumentException("Project not found");

        var tasks = await _taskRepository.GetByProjectAsync(projectId, ct);
        var teamMembers = await _context.ProjectMembers
            .Where(pm => pm.ProjectId == projectId)
            .Include(pm => pm.User)
            .ToListAsync(ct);

        var memberPerformances = new List<TeamMemberPerformanceDto>();

        foreach (var member in teamMembers)
        {
            var memberTasks = tasks.Where(t => t.AssignedToId == member.UserId).ToList();
            var completedTasks = memberTasks.Where(t => t.Status == TaskStatus.Done).ToList();
            var inProgressTasks = memberTasks.Where(t => t.Status == TaskStatus.InProgress).ToList();

            var productivityScore = CalculateProductivityScore(completedTasks, memberTasks.Count);
            var qualityScore = CalculateQualityScore(completedTasks);
            var averageCompletionTime = CalculateAverageTaskDuration(completedTasks);
            var utilizationRate = CalculateUtilizationRate(member.User, memberTasks.Count);

            memberPerformances.Add(new TeamMemberPerformanceDto(
                member.UserId,
                member.User.FullName,
                completedTasks.Count,
                inProgressTasks.Count,
                productivityScore,
                qualityScore,
                averageCompletionTime,
                member.User.CurrentWorkload,
                utilizationRate
            ));
        }

        var averageProductivity = memberPerformances.Any() ? memberPerformances.Average(m => m.ProductivityScore) : 0;
        var averageQuality = memberPerformances.Any() ? memberPerformances.Average(m => m.QualityScore) : 0;
        var totalTasksCompleted = memberPerformances.Sum(m => m.TasksCompleted);
        var averageCompletionTime = memberPerformances.Any() ? memberPerformances.Average(m => m.AverageCompletionTime) : 0;

        return new TeamPerformanceDto(
            projectId,
            memberPerformances,
            averageProductivity,
            averageQuality,
            totalTasksCompleted,
            averageCompletionTime
        );
    }

    public async Task<EmployeeProductivityDto> GetEmployeeProductivityAsync(Guid employeeId, CancellationToken ct = default)
    {
        var employee = await _context.UserProfiles.FindAsync(new object[] { employeeId }, ct);
        if (employee == null) throw new ArgumentException("Employee not found");

        var tasks = await _context.Tasks
            .Where(t => t.AssignedToId == employeeId)
            .ToListAsync(ct);

        var completedTasks = tasks.Where(t => t.Status == TaskStatus.Done).ToList();
        var completionRate = tasks.Count > 0 ? (double)completedTasks.Count / tasks.Count * 100 : 0;
        var averageDuration = CalculateAverageTaskDuration(completedTasks);
        var performanceScore = employee.PerformanceScore;

        var recentTasks = completedTasks
            .OrderByDescending(t => t.UpdatedAt)
            .Take(10)
            .Select(t => new TaskPerformanceDto(
                t.Id,
                t.Title,
                t.UpdatedAt,
                CalculateTaskDuration(t),
                0.8 // Quality score would be calculated based on feedback/quality metrics
            ));

        return new EmployeeProductivityDto(
            employeeId,
            employee.FullName,
            tasks.Count,
            completedTasks.Count,
            completionRate,
            averageDuration,
            performanceScore,
            recentTasks
        );
    }

    public async Task<BottleneckAnalysisDto> GetBottleneckAnalysisAsync(Guid projectId, CancellationToken ct = default)
    {
        var tasks = await _taskRepository.GetByProjectAsync(projectId, ct);
        var bottlenecks = new List<BottleneckDto>();

        // Analyze overdue tasks as bottlenecks
        var overdueTasks = tasks.Where(t => t.DueDate.HasValue && t.DueDate.Value < DateTime.UtcNow && t.Status != TaskStatus.Done).ToList();
        if (overdueTasks.Any())
        {
            bottlenecks.Add(new BottleneckDto(
                "Schedule",
                "Tasks are consistently missing deadlines",
                overdueTasks.Count,
                CalculateBottleneckImpact(overdueTasks.Count, tasks.Count),
                "High"
            ));
        }

        // Analyze high-priority tasks stuck in progress
        var stuckHighPriorityTasks = tasks.Where(t => 
            t.Priority == TaskPriority.Critical && 
            t.Status == TaskStatus.InProgress && 
            t.CreatedAt < DateTime.UtcNow.AddDays(-7)).ToList();

        if (stuckHighPriorityTasks.Any())
        {
            bottlenecks.Add(new BottleneckDto(
                "Priority",
                "Critical tasks are taking too long to complete",
                stuckHighPriorityTasks.Count,
                CalculateBottleneckImpact(stuckHighPriorityTasks.Count, tasks.Count),
                "Critical"
            ));
        }

        // Analyze workload distribution
        var teamMembers = await _context.ProjectMembers
            .Where(pm => pm.ProjectId == projectId)
            .Include(pm => pm.User)
            .ToListAsync(ct);

        var overloadedMembers = teamMembers.Where(pm => pm.User.CurrentWorkload > 40).ToList();
        if (overloadedMembers.Any())
        {
            bottlenecks.Add(new BottleneckDto(
                "Workload",
                "Team members are overloaded",
                overloadedMembers.Count,
                CalculateBottleneckImpact(overloadedMembers.Count, teamMembers.Count),
                "Medium"
            ));
        }

        var primaryBottleneck = bottlenecks.OrderByDescending(b => b.ImpactScore).FirstOrDefault()?.Type ?? "None";
        var recommendations = GenerateBottleneckRecommendations(bottlenecks);

        return new BottleneckAnalysisDto(
            projectId,
            bottlenecks,
            primaryBottleneck,
            recommendations
        );
    }

    public async Task<CompletionMetricsDto> GetCompletionMetricsAsync(Guid projectId, CancellationToken ct = default)
    {
        var tasks = await _taskRepository.GetByProjectAsync(projectId, ct);
        var completedTasks = tasks.Where(t => t.Status == TaskStatus.Done).ToList();

        var onTimeTasks = completedTasks.Where(t => !t.DueDate.HasValue || t.UpdatedAt <= t.DueDate.Value).ToList();
        var lateTasks = completedTasks.Where(t => t.DueDate.HasValue && t.UpdatedAt > t.DueDate.Value).ToList();

        var onTimeRate = completedTasks.Any() ? (double)onTimeTasks.Count / completedTasks.Count * 100 : 0;
        var averageDelay = lateTasks.Any() ? lateTasks.Average(t => (t.UpdatedAt - t.DueDate!.Value).TotalDays) : 0;

        var monthlyTrends = CalculateMonthlyCompletionTrends(completedTasks);

        return new CompletionMetricsDto(
            projectId,
            onTimeTasks.Count,
            lateTasks.Count,
            onTimeRate,
            averageDelay,
            monthlyTrends
        );
    }

    public async Task<IEnumerable<RiskAlertDto>> GetRiskAlertsAsync(Guid projectId, CancellationToken ct = default)
    {
        var alerts = new List<RiskAlertDto>();
        var tasks = await _taskRepository.GetByProjectAsync(projectId, ct);

        // Check for overdue critical tasks
        var overdueCriticalTasks = tasks.Where(t => 
            t.Priority == TaskPriority.Critical && 
            t.DueDate.HasValue && 
            t.DueDate.Value < DateTime.UtcNow && 
            t.Status != TaskStatus.Done).ToList();

        if (overdueCriticalTasks.Any())
        {
            alerts.Add(new RiskAlertDto(
                projectId,
                "Schedule",
                $"{overdueCriticalTasks.Count} critical tasks are overdue",
                "Critical",
                overdueCriticalTasks.Count,
                DateTime.UtcNow,
                new[] { "Immediately address overdue tasks", "Reallocate resources", "Review project timeline" }
            ));
        }

        // Check for approaching deadlines
        var approachingDeadlines = tasks.Where(t => 
            t.DueDate.HasValue && 
            t.DueDate.Value <= DateTime.UtcNow.AddDays(3) && 
            t.DueDate.Value > DateTime.UtcNow && 
            t.Status != TaskStatus.Done).ToList();

        if (approachingDeadlines.Any())
        {
            alerts.Add(new RiskAlertDto(
                projectId,
                "Deadline",
                $"{approachingDeadlines.Count} tasks have deadlines within 3 days",
                "High",
                approachingDeadlines.Count,
                DateTime.UtcNow,
                new[] { "Prioritize tasks with approaching deadlines", "Check resource availability", "Consider extending deadlines if necessary" }
            ));
        }

        return alerts;
    }

    public async Task<WorkloadReportDto> GetWorkloadReportAsync(Guid projectId, CancellationToken ct = default)
    {
        var teamMembers = await _context.ProjectMembers
            .Where(pm => pm.ProjectId == projectId)
            .Include(pm => pm.User)
            .ToListAsync(ct);

        var tasks = await _taskRepository.GetByProjectAsync(projectId, ct);

        var employeeWorkloads = new List<EmployeeWorkloadDto>();

        foreach (var member in teamMembers)
        {
            var memberTasks = tasks.Where(t => t.AssignedToId == member.UserId).ToList();
            var currentWorkload = member.User.CurrentWorkload;
            var recommendedWorkload = 40.0; // 40 hours per week
            var utilizationRate = currentWorkload / recommendedWorkload * 100;
            var workloadStatus = GetWorkloadStatus(utilizationRate);
            var recommendations = GenerateWorkloadRecommendations(utilizationRate, memberTasks.Count);

            employeeWorkloads.Add(new EmployeeWorkloadDto(
                member.UserId,
                member.User.FullName,
                currentWorkload,
                recommendedWorkload,
                utilizationRate,
                workloadStatus,
                recommendations
            ));
        }

        var totalWorkload = employeeWorkloads.Sum(w => w.CurrentWorkload);
        var averageWorkload = employeeWorkloads.Any() ? employeeWorkloads.Average(w => w.CurrentWorkload) : 0;
        var overloadedEmployees = employeeWorkloads.Count(w => w.WorkloadStatus == "Overloaded");
        var underutilizedEmployees = employeeWorkloads.Count(w => w.WorkloadStatus == "Underutilized");

        return new WorkloadReportDto(
            projectId,
            employeeWorkloads,
            totalWorkload,
            averageWorkload,
            overloadedEmployees,
            underutilizedEmployees
        );
    }

    private double CalculateAverageTaskDuration(IEnumerable<TaskItem> tasks)
    {
        if (!tasks.Any()) return 0;

        var durations = tasks.Select(t => CalculateTaskDuration(t)).Where(d => d > 0);
        return durations.Any() ? durations.Average() : 0;
    }

    private double CalculateTaskDuration(TaskItem task)
    {
        var endTime = task.Status == TaskStatus.Done ? task.UpdatedAt : DateTime.UtcNow;
        return (endTime - task.CreatedAt).TotalDays;
    }

    private DateTime? CalculateProjectedCompletionDate(ProjectItem project, IEnumerable<TaskItem> tasks, int completedCount)
    {
        if (!tasks.Any() || completedCount == 0) return null;

        var averageCompletionRate = (double)completedCount / project.CreatedAt.Subtract(DateTime.UtcNow).TotalDays;
        var remainingTasks = tasks.Count() - completedCount;
        var daysToComplete = remainingTasks / averageCompletionRate;

        return DateTime.UtcNow.AddDays(daysToComplete);
    }

    private double CalculateProductivityScore(IEnumerable<TaskItem> completedTasks, int totalTasks)
    {
        if (totalTasks == 0) return 0;
        return (double)completedTasks.Count() / totalTasks;
    }

    private double CalculateQualityScore(IEnumerable<TaskItem> completedTasks)
    {
        // Quality score would be calculated based on feedback, rework, etc.
        // For now, return a default score
        return 0.8;
    }

    private double CalculateUtilizationRate(UserProfile employee, int taskCount)
    {
        var recommendedTasks = 5; // Recommended number of active tasks per employee
        return Math.Min((double)taskCount / recommendedTasks * 100, 100);
    }

    private double CalculateBottleneckImpact(int affectedItems, int totalItems)
    {
        return totalItems > 0 ? (double)affectedItems / totalItems : 0;
    }

    private string[] GenerateBottleneckRecommendations(IEnumerable<BottleneckDto> bottlenecks)
    {
        var recommendations = new List<string>();

        foreach (var bottleneck in bottlenecks)
        {
            switch (bottleneck.Type)
            {
                case "Schedule":
                    recommendations.Add("Review and adjust project timeline");
                    recommendations.Add("Consider adding more resources");
                    break;
                case "Priority":
                    recommendations.Add("Re-prioritize critical tasks");
                    recommendations.Add("Remove blockers for critical path items");
                    break;
                case "Workload":
                    recommendations.Add("Redistribute tasks among team members");
                    recommendations.Add("Consider hiring additional resources");
                    break;
            }
        }

        return recommendations.Distinct().ToArray();
    }

    private IEnumerable<MonthlyCompletionDto> CalculateMonthlyCompletionTrends(IEnumerable<TaskItem> completedTasks)
    {
        return completedTasks
            .GroupBy(t => new { t.UpdatedAt.Year, t.UpdatedAt.Month })
            .Select(g => new MonthlyCompletionDto(
                g.Key.Month,
                g.Key.Year,
                g.Count(),
                0 // Completion rate would be calculated against monthly targets
            ))
            .OrderBy(m => m.Year)
            .ThenBy(m => m.Month);
    }

    private string GetWorkloadStatus(double utilizationRate)
    {
        return utilizationRate switch
        {
            < 50 => "Underutilized",
            < 90 => "Optimal",
            < 110 => "Heavy",
            _ => "Overloaded"
        };
    }

    private string[] GenerateWorkloadRecommendations(double utilizationRate, int taskCount)
    {
        var recommendations = new List<string>();

        if (utilizationRate < 50)
        {
            recommendations.Add("Consider assigning more tasks");
            recommendations.Add("Look for opportunities to contribute to other projects");
        }
        else if (utilizationRate > 110)
        {
            recommendations.Add("Redistribute some tasks to other team members");
            recommendations.Add("Consider extending deadlines for non-critical tasks");
        }
        else if (utilizationRate > 90)
        {
            recommendations.Add("Monitor workload closely");
            recommendations.Add("Prioritize tasks to focus on most important ones");
        }

        return recommendations.ToArray();
    }
}
