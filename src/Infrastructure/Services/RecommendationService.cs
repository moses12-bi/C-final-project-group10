using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Core.Services;
using Core.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class RecommendationService : IRecommendationService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IRecommendationRepository _recommendationRepository;
    private readonly AppDbContext _context;

    public RecommendationService(
        ITaskRepository taskRepository,
        IProjectRepository projectRepository,
        IRecommendationRepository recommendationRepository,
        AppDbContext context)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _recommendationRepository = recommendationRepository;
        _context = context;
    }

    public async Task<IEnumerable<EmployeeRecommendationDto>> GetBestEmployeesForTaskAsync(Guid taskId, CancellationToken ct = default)
    {
        var task = await _taskRepository.GetAsync(taskId, ct);
        if (task == null) return Enumerable.Empty<EmployeeRecommendationDto>();

        var projectMembers = await _context.ProjectMembers
            .Where(pm => pm.ProjectId == task.ProjectId)
            .Include(pm => pm.User)
            .ToListAsync(ct);

        var recommendations = new List<EmployeeRecommendationDto>();

        foreach (var member in projectMembers)
        {
            var score = await CalculateEmployeeScoreAsync(member.User, task, ct);
            var rationale = GenerateRationale(member.User, task, score);

            recommendations.Add(new EmployeeRecommendationDto(
                member.UserId,
                member.User.FullName,
                score.TotalScore,
                rationale,
                score.SkillsMatch,
                score.WorkloadAvailability,
                score.PastPerformance,
                score.CurrentWorkload
            ));
        }

        return recommendations.OrderByDescending(r => r.Score);
    }

    public async Task<IEnumerable<TeamMemberRecommendationDto>> GetBestTeamMembersForProjectAsync(Guid projectId, CancellationToken ct = default)
    {
        var project = await _projectRepository.GetAsync(projectId, ct);
        if (project == null) return Enumerable.Empty<TeamMemberRecommendationDto>();

        var allEmployees = await _context.UserProfiles
            .Where(u => u.Role != UserRole.Manager)
            .ToListAsync(ct);

        var recommendations = new List<TeamMemberRecommendationDto>();

        foreach (var employee in allEmployees)
        {
            var score = await CalculateTeamMemberScoreAsync(employee, project, ct);
            var matchingSkills = GetMatchingSkills(employee, project);

            recommendations.Add(new TeamMemberRecommendationDto(
                employee.Id,
                employee.FullName,
                score,
                $"Score: {score:F2} based on skills and availability",
                matchingSkills
            ));
        }

        return recommendations.OrderByDescending(r => r.Score);
    }

    public async Task<DeadlineRecommendationDto> GetEstimatedDeadlineAsync(Guid taskId, CancellationToken ct = default)
    {
        var task = await _taskRepository.GetAsync(taskId, ct);
        if (task == null) return new DeadlineRecommendationDto(DateTime.UtcNow.AddDays(7), 7, 0.5, Array.Empty<string>());

        var estimatedDays = CalculateEstimatedDays(task);
        var completionDate = DateTime.UtcNow.AddDays(estimatedDays);
        var confidence = CalculateConfidence(task);
        var riskFactors = IdentifyRiskFactors(task);

        return new DeadlineRecommendationDto(
            completionDate,
            estimatedDays,
            confidence,
            riskFactors
        );
    }

    public async Task<RiskAssessmentDto> AssessProjectRiskAsync(Guid projectId, CancellationToken ct = default)
    {
        var project = await _projectRepository.GetAsync(projectId, ct);
        if (project == null) return new RiskAssessmentDto(0.0, "Low", Array.Empty<string>(), Array.Empty<string>());

        var riskScore = await CalculateProjectRiskScoreAsync(project, ct);
        var riskLevel = GetRiskLevel(riskScore);
        var riskFactors = IdentifyProjectRiskFactors(project);
        var mitigationStrategies = GenerateMitigationStrategies(riskFactors);

        return new RiskAssessmentDto(
            riskScore,
            riskLevel,
            riskFactors,
            mitigationStrategies
        );
    }

    public async Task<WorkloadImpactDto> AssessWorkloadImpactAsync(Guid employeeId, Guid taskId, CancellationToken ct = default)
    {
        var employee = await _context.UserProfiles.FindAsync(new object[] { employeeId }, ct);
        var task = await _taskRepository.GetAsync(taskId, ct);

        if (employee == null || task == null)
            return new WorkloadImpactDto(0, 0, 0, false, "Insufficient data");

        var currentWorkload = employee.CurrentWorkload;
        var projectedWorkload = currentWorkload + task.EstimatedEffort;
        var impactPercentage = (projectedWorkload / currentWorkload) * 100;
        var isOverloaded = projectedWorkload > 40; // 40 hours per week threshold
        var recommendation = isOverloaded ? "Consider redistributing workload" : "Workload is manageable";

        return new WorkloadImpactDto(
            currentWorkload,
            projectedWorkload,
            impactPercentage,
            isOverloaded,
            recommendation
        );
    }

    public async Task<double> CalculateTaskDifficultyAsync(CreateTaskRequest taskRequest, CancellationToken ct = default)
    {
        var difficulty = taskRequest.DifficultyScore * 0.4;
        var effortFactor = Math.Min(taskRequest.EstimatedEffort / 20, 1.0) * 0.3;
        var priorityFactor = ((int)taskRequest.Priority) * 0.2;
        var dependencyFactor = taskRequest.DependencyTaskId.HasValue ? 0.1 : 0;

        return difficulty + effortFactor + priorityFactor + dependencyFactor;
    }

    private async Task<EmployeeScore> CalculateEmployeeScoreAsync(UserProfile employee, TaskItem task, CancellationToken ct)
    {
        var skillsMatch = await CalculateSkillsMatchAsync(employee, task, ct);
        var workloadAvailability = CalculateWorkloadAvailability(employee);
        var pastPerformance = employee.PerformanceScore;
        var currentWorkload = employee.CurrentWorkload;

        var totalScore = (skillsMatch * 0.4) + (workloadAvailability * 0.3) + (pastPerformance * 0.2) + (currentWorkload * 0.1);

        return new EmployeeScore(totalScore, skillsMatch, workloadAvailability, pastPerformance, currentWorkload);
    }

    private async Task<double> CalculateSkillsMatchAsync(UserProfile employee, TaskItem task, CancellationToken ct)
    {
        // Simplified skills matching - in real implementation, this would parse skills JSON
        var employeeSkills = new[] { "C#", "SQL", "JavaScript" }; // Placeholder
        var requiredSkills = new[] { "C#", "SQL" }; // Placeholder based on task

        var matchingSkills = employeeSkills.Intersect(requiredSkills).Count();
        return (double)matchingSkills / requiredSkills.Length;
    }

    private double CalculateWorkloadAvailability(UserProfile employee)
    {
        var maxWorkload = 40.0; // 40 hours per week
        return Math.Max(0, (maxWorkload - employee.CurrentWorkload) / maxWorkload);
    }

    private string GenerateRationale(UserProfile employee, TaskItem task, EmployeeScore score)
    {
        var reasons = new List<string>();

        if (score.SkillsMatch > 0.7) reasons.Add("Strong skills match");
        if (score.WorkloadAvailability > 0.5) reasons.Add("Good availability");
        if (score.PastPerformance > 0.7) reasons.Add("Excellent past performance");

        return reasons.Count > 0 ? string.Join(", ", reasons) : "Basic match";
    }

    private async Task<double> CalculateTeamMemberScoreAsync(UserProfile employee, Project project, CancellationToken ct)
    {
        var skillsMatch = 0.8; // Placeholder
        var availability = CalculateWorkloadAvailability(employee);
        var performance = employee.PerformanceScore;

        return (skillsMatch * 0.4) + (availability * 0.3) + (performance * 0.3);
    }

    private string[] GetMatchingSkills(UserProfile employee, Project project)
    {
        return new[] { "C#", "SQL", "Project Management" }; // Placeholder
    }

    private int CalculateEstimatedDays(TaskItem task)
    {
        var baseDays = (int)(task.EstimatedEffort / 8); // 8 hours per day
        var complexityMultiplier = task.Priority == TaskPriority.Critical ? 1.5 : 1.0;
        return (int)(baseDays * complexityMultiplier);
    }

    private double CalculateConfidence(TaskItem task)
    {
        var confidence = 0.8; // Base confidence
        if (task.DifficultyScore > 7) confidence -= 0.2;
        if (task.Priority == TaskPriority.Critical) confidence -= 0.1;
        return Math.Max(0.3, confidence);
    }

    private string[] IdentifyRiskFactors(TaskItem task)
    {
        var risks = new List<string>();
        if (task.DifficultyScore > 8) risks.Add("High complexity");
        if (task.Priority == TaskPriority.Critical) risks.Add("Critical priority");
        if (task.DueDate.HasValue && task.DueDate.Value < DateTime.UtcNow.AddDays(3)) risks.Add("Tight deadline");
        return risks.ToArray();
    }

    private async Task<double> CalculateProjectRiskScoreAsync(Project project, CancellationToken ct)
    {
        var tasks = await _context.Tasks.Where(t => t.ProjectId == project.Id).ToListAsync(ct);
        var overdueTasks = tasks.Count(t => t.DueDate.HasValue && t.DueDate.Value < DateTime.UtcNow);
        var highPriorityTasks = tasks.Count(t => t.Priority == TaskPriority.Critical);
        var totalTasks = tasks.Count;

        var overdueRatio = totalTasks > 0 ? (double)overdueTasks / totalTasks : 0;
        var highPriorityRatio = totalTasks > 0 ? (double)highPriorityTasks / totalTasks : 0;

        return (overdueRatio * 0.5) + (highPriorityRatio * 0.3) + (project.RiskLevel * 0.2);
    }

    private string GetRiskLevel(double riskScore)
    {
        return riskScore switch
        {
            < 0.3 => "Low",
            < 0.6 => "Medium",
            < 0.8 => "High",
            _ => "Critical"
        };
    }

    private string[] IdentifyProjectRiskFactors(Project project)
    {
        var factors = new List<string>();
        if (project.RiskLevel > 0.7) factors.Add("High project risk level");
        if (project.EndDate.HasValue && project.EndDate.Value < DateTime.UtcNow.AddDays(30)) factors.Add("Approaching deadline");
        return factors.ToArray();
    }

    private string[] GenerateMitigationStrategies(string[] riskFactors)
    {
        var strategies = new List<string>();
        foreach (var factor in riskFactors)
        {
            if (factor.Contains("deadline")) strategies.Add("Reallocate resources to critical path");
            if (factor.Contains("risk")) strategies.Add("Implement risk monitoring");
            if (factor.Contains("overdue")) strategies.Add("Review task estimates and dependencies");
        }
        return strategies.ToArray();
    }

    private record EmployeeScore(
        double TotalScore,
        double SkillsMatch,
        double WorkloadAvailability,
        double PastPerformance,
        double CurrentWorkload
    );
}
