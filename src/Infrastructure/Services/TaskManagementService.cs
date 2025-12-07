using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Core.Services;
using Core.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class TaskManagementService : ITaskManagementService
{
    private readonly ITaskRepository _taskRepository;
    private readonly ITaskUpdateRepository _taskUpdateRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IRecommendationService _recommendationService;
    private readonly INotificationRepository _notificationRepository;
    private readonly AppDbContext _context;

    public TaskManagementService(
        ITaskRepository taskRepository,
        ITaskUpdateRepository taskUpdateRepository,
        IProjectRepository projectRepository,
        IRecommendationService recommendationService,
        INotificationRepository notificationRepository,
        AppDbContext context)
    {
        _taskRepository = taskRepository;
        _taskUpdateRepository = taskUpdateRepository;
        _projectRepository = projectRepository;
        _recommendationService = recommendationService;
        _notificationRepository = notificationRepository;
        _context = context;
    }

    public async Task<TaskItem> CreateTaskAsync(CreateTaskRequest request, CancellationToken ct = default)
    {
        // Validate project exists and user has access
        var project = await _projectRepository.GetAsync(request.ProjectId, ct);
        if (project == null)
            throw new ArgumentException("Project not found");

        // Create the task
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            ProjectId = request.ProjectId,
            CreatedById = request.CreatedById,
            AssignedToId = request.AssignedToId,
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            Status = TaskStatus.ToDo,
            EstimatedEffort = request.EstimatedEffort,
            DueDate = request.DueDate,
            DifficultyScore = request.DifficultyScore,
            DependencyTaskId = request.DependencyTaskId,
            CreatedAt = DateTime.UtcNow
        };

        await _taskRepository.AddAsync(task, ct);

        // Create notification for assigned user if applicable
        if (request.AssignedToId.HasValue)
        {
            await CreateTaskAssignmentNotificationAsync(task, request.AssignedToId.Value, ct);
        }

        return task;
    }

    public async Task<TaskItem> UpdateTaskStatusAsync(Guid taskId, TaskStatus newStatus, Guid updatedById, string? comment = null, CancellationToken ct = default)
    {
        var task = await _taskRepository.GetAsync(taskId, ct);
        if (task == null)
            throw new ArgumentException("Task not found");

        var oldStatus = task.Status;
        task.Status = newStatus;

        await _taskRepository.UpdateAsync(task, ct);

        // Add task update
        var update = new TaskUpdate
        {
            Id = Guid.NewGuid(),
            TaskItemId = taskId,
            UpdatedById = updatedById,
            Comment = comment ?? $"Status changed from {oldStatus} to {newStatus}",
            Status = newStatus,
            CreatedAt = DateTime.UtcNow,
            EffortLogged = 0
        };

        await _taskUpdateRepository.AddAsync(update, ct);

        // Create notifications
        await CreateTaskStatusUpdateNotificationAsync(task, oldStatus, newStatus, updatedById, ct);

        return task;
    }

    public async Task<TaskItem> AssignTaskAsync(Guid taskId, Guid assignedToId, CancellationToken ct = default)
    {
        var task = await _taskRepository.GetAsync(taskId, ct);
        if (task == null)
            throw new ArgumentException("Task not found");

        var previousAssigneeId = task.AssignedToId;
        task.AssignedToId = assignedToId;

        await _taskRepository.UpdateAsync(task, ct);

        // Create notification for new assignment
        await CreateTaskAssignmentNotificationAsync(task, assignedToId, ct);

        // If there was a previous assignee, notify them of reassignment
        if (previousAssigneeId.HasValue && previousAssigneeId != assignedToId)
        {
            await CreateTaskReassignmentNotificationAsync(task, previousAssigneeId.Value, ct);
        }

        return task;
    }

    public async Task<IEnumerable<TaskItem>> GetTasksByProjectAsync(Guid projectId, CancellationToken ct = default)
    {
        var tasks = await _taskRepository.GetByProjectAsync(projectId, ct);
        return tasks.OrderBy(t => t.DueDate).ThenBy(t => t.Priority);
    }

    public async Task<IEnumerable<TaskItem>> GetTasksByEmployeeAsync(Guid employeeId, CancellationToken ct = default)
    {
        var tasks = await _context.Tasks
            .Where(t => t.AssignedToId == employeeId)
            .Include(t => t.Project)
            .OrderBy(t => t.DueDate)
            .ThenBy(t => t.Priority)
            .ToListAsync(ct);

        return tasks;
    }

    public async Task<TaskUpdate> AddTaskUpdateAsync(Guid taskId, Guid updatedById, string comment, double effortLogged, CancellationToken ct = default)
    {
        var task = await _taskRepository.GetAsync(taskId, ct);
        if (task == null)
            throw new ArgumentException("Task not found");

        var update = new TaskUpdate
        {
            Id = Guid.NewGuid(),
            TaskItemId = taskId,
            UpdatedById = updatedById,
            Comment = comment,
            Status = task.Status,
            CreatedAt = DateTime.UtcNow,
            EffortLogged = effortLogged
        };

        await _taskUpdateRepository.AddAsync(update, ct);

        // Create notification for task update
        await CreateTaskUpdateNotificationAsync(task, update, ct);

        return update;
    }

    public async Task<IEnumerable<TaskUpdate>> GetTaskUpdatesAsync(Guid taskId, CancellationToken ct = default)
    {
        return await _taskUpdateRepository.GetByTaskAsync(taskId, ct);
    }

    public async Task<bool> CanUserAccessTaskAsync(Guid userId, Guid taskId, CancellationToken ct = default)
    {
        var task = await _taskRepository.GetAsync(taskId, ct);
        if (task == null) return false;

        // Check if user is assigned to task, created the task, or is a manager/team lead of the project
        var userProfile = await _context.UserProfiles.FindAsync(new object[] { userId }, ct);
        if (userProfile == null) return false;

        if (task.AssignedToId == userId || task.CreatedById == userId) return true;

        var project = await _projectRepository.GetAsync(task.ProjectId, ct);
        if (project == null) return false;

        return project.ManagerId == userId || userProfile.Role == UserRole.Manager || userProfile.Role == UserRole.TeamLead;
    }

    public async Task<IEnumerable<TaskItem>> GetOverdueTasksAsync(CancellationToken ct = default)
    {
        var overdueTasks = await _context.Tasks
            .Where(t => t.DueDate.HasValue && t.DueDate.Value < DateTime.UtcNow && t.Status != TaskStatus.Done)
            .Include(t => t.Project)
            .Include(t => t.AssignedTo)
            .OrderBy(t => t.DueDate)
            .ToListAsync(ct);

        return overdueTasks;
    }

    public async Task<IEnumerable<TaskItem>> GetCriticalTasksAsync(CancellationToken ct = default)
    {
        var criticalTasks = await _context.Tasks
            .Where(t => t.Priority == TaskPriority.Critical && t.Status != TaskStatus.Done)
            .Include(t => t.Project)
            .Include(t => t.AssignedTo)
            .OrderBy(t => t.DueDate)
            .ToListAsync(ct);

        return criticalTasks;
    }

    private async Task CreateTaskAssignmentNotificationAsync(TaskItem task, Guid assignedToId, CancellationToken ct)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = assignedToId,
            Type = "TaskAssigned",
            PayloadJson = System.Text.Json.JsonSerializer.Serialize(new
            {
                TaskId = task.Id,
                TaskTitle = task.Title,
                ProjectId = task.ProjectId,
                AssignedAt = DateTime.UtcNow
            }),
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _notificationRepository.AddAsync(notification, ct);
    }

    private async Task CreateTaskStatusUpdateNotificationAsync(TaskItem task, TaskStatus oldStatus, TaskStatus newStatus, Guid updatedById, CancellationToken ct)
    {
        // Notify project manager and task creator
        var project = await _projectRepository.GetAsync(task.ProjectId, ct);
        var notificationRecipients = new List<Guid> { project!.ManagerId };

        if (task.CreatedById != project.ManagerId)
            notificationRecipients.Add(task.CreatedById.Value);

        if (task.AssignedToId.HasValue && !notificationRecipients.Contains(task.AssignedToId.Value))
            notificationRecipients.Add(task.AssignedToId.Value);

        foreach (var recipientId in notificationRecipients)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = recipientId,
                Type = "TaskStatusUpdated",
                PayloadJson = System.Text.Json.JsonSerializer.Serialize(new
                {
                    TaskId = task.Id,
                    TaskTitle = task.Title,
                    OldStatus = oldStatus.ToString(),
                    NewStatus = newStatus.ToString(),
                    UpdatedById = updatedById,
                    UpdatedAt = DateTime.UtcNow
                }),
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notification, ct);
        }
    }

    private async Task CreateTaskReassignmentNotificationAsync(TaskItem task, Guid previousAssigneeId, CancellationToken ct)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = previousAssigneeId,
            Type = "TaskReassigned",
            PayloadJson = System.Text.Json.JsonSerializer.Serialize(new
            {
                TaskId = task.Id,
                TaskTitle = task.Title,
                ReassignedAt = DateTime.UtcNow
            }),
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _notificationRepository.AddAsync(notification, ct);
    }

    private async Task CreateTaskUpdateNotificationAsync(TaskItem task, TaskUpdate update, CancellationToken ct = default)
    {
        // Notify relevant stakeholders about the update
        var project = await _projectRepository.GetAsync(task.ProjectId, ct);
        var notificationRecipients = new List<Guid> { project!.ManagerId };

        if (task.CreatedById != project.ManagerId)
            notificationRecipients.Add(task.CreatedById.Value);

        if (task.AssignedToId.HasValue && !notificationRecipients.Contains(task.AssignedToId.Value))
            notificationRecipients.Add(task.AssignedToId.Value);

        foreach (var recipientId in notificationRecipients.Distinct())
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = recipientId,
                Type = "TaskUpdated",
                PayloadJson = System.Text.Json.JsonSerializer.Serialize(new
                {
                    TaskId = task.Id,
                    TaskTitle = task.Title,
                    UpdateId = update.Id,
                    UpdatedById = update.UpdatedById,
                    UpdatedAt = update.CreatedAt
                }),
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notification, ct);
        }
    }
}
