using Core.Interfaces;
using Core.Models;
using Core.Services;
using Core.DTOs;
using Microsoft.AspNetCore.SignalR;
using Web.Hubs;
using System.Text.Json;

namespace Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly AppDbContext _context;

    public NotificationService(
        INotificationRepository notificationRepository,
        IHubContext<NotificationHub> hubContext,
        AppDbContext context)
    {
        _notificationRepository = notificationRepository;
        _hubContext = hubContext;
        _context = context;
    }

    public async Task<Notification> CreateNotificationAsync(CreateNotificationRequest request, CancellationToken ct = default)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Type = request.Type,
            PayloadJson = request.PayloadJson,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _notificationRepository.AddAsync(notification, ct);

        // Send real-time notification
        await SendRealTimeNotificationAsync(request.UserId, request.PayloadJson, request.Type, ct);

        return notification;
    }

    public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId, CancellationToken ct = default)
    {
        return await _notificationRepository.GetForUserAsync(userId, ct);
    }

    public async Task MarkNotificationAsReadAsync(Guid notificationId, CancellationToken ct = default)
    {
        await _notificationRepository.MarkAsReadAsync(notificationId, ct);
    }

    public async Task MarkAllNotificationsAsReadAsync(Guid userId, CancellationToken ct = default)
    {
        var notifications = await _notificationRepository.GetForUserAsync(userId, ct);
        foreach (var notification in notifications.Where(n => !n.IsRead))
        {
            await _notificationRepository.MarkAsReadAsync(notification.Id, ct);
        }
    }

    public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct = default)
    {
        var notifications = await _notificationRepository.GetForUserAsync(userId, ct);
        return notifications.Count(n => !n.IsRead);
    }

    public async Task SendRealTimeNotificationAsync(Guid userId, string message, string type = "general", CancellationToken ct = default)
    {
        await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", new
        {
            Type = type,
            Message = message,
            Timestamp = DateTime.UtcNow,
            UserId = userId
        });
    }

    public async Task SendProjectNotificationAsync(Guid projectId, string message, string type = "project", CancellationToken ct = default)
    {
        await _hubContext.Clients.Group($"Project_{projectId}").SendAsync("ReceiveNotification", new
        {
            Type = type,
            Message = message,
            ProjectId = projectId,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task SendTaskNotificationAsync(Guid taskId, string message, string type = "task", CancellationToken ct = default)
    {
        // Get task to find project for group notification
        var task = await _context.Tasks.FindAsync(new object[] { taskId }, ct);
        if (task != null)
        {
            await _hubContext.Clients.Group($"Project_{task.ProjectId}").SendAsync("ReceiveNotification", new
            {
                Type = type,
                Message = message,
                TaskId = taskId,
                ProjectId = task.ProjectId,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    // Helper methods for creating specific notification types
    public async Task CreateTaskAssignedNotificationAsync(Guid taskId, Guid assignedToId, CancellationToken ct = default)
    {
        var task = await _context.Tasks.FindAsync(new object[] { taskId }, ct);
        if (task == null) return;

        var notification = new CreateNotificationRequest(
            assignedToId,
            "TaskAssigned",
            JsonSerializer.Serialize(new
            {
                TaskId = taskId,
                TaskTitle = task.Title,
                AssignedAt = DateTime.UtcNow
            })
        );

        await CreateNotificationAsync(notification, ct);
    }

    public async Task CreateTaskStatusUpdateNotificationAsync(Guid taskId, string oldStatus, string newStatus, Guid updatedById, CancellationToken ct = default)
    {
        var task = await _context.Tasks.FindAsync(new object[] { taskId }, ct);
        if (task == null) return;

        // Notify relevant stakeholders
        var project = await _context.Projects.FindAsync(new object[] { task.ProjectId }, ct);
        var notificationRecipients = new List<Guid> { project!.ManagerId };

        if (task.CreatedById != project.ManagerId)
            notificationRecipients.Add(task.CreatedById.Value);

        if (task.AssignedToId.HasValue && !notificationRecipients.Contains(task.AssignedToId.Value))
            notificationRecipients.Add(task.AssignedToId.Value);

        foreach (var recipientId in notificationRecipients.Distinct())
        {
            var notification = new CreateNotificationRequest(
                recipientId,
                "TaskStatusUpdated",
                JsonSerializer.Serialize(new
                {
                    TaskId = taskId,
                    TaskTitle = task.Title,
                    OldStatus = oldStatus,
                    NewStatus = newStatus,
                    UpdatedById = updatedById,
                    UpdatedAt = DateTime.UtcNow
                })
            );

            await CreateNotificationAsync(notification, ct);
        }
    }

    public async Task CreateOverdueTaskNotificationAsync(Guid taskId, CancellationToken ct = default)
    {
        var task = await _context.Tasks.FindAsync(new object[] { taskId }, ct);
        if (task == null || !task.AssignedToId.HasValue) return;

        var notification = new CreateNotificationRequest(
            task.AssignedToId.Value,
            "TaskOverdue",
            JsonSerializer.Serialize(new
            {
                TaskId = taskId,
                TaskTitle = task.Title,
                DueDate = task.DueDate,
                OverdueSince = DateTime.UtcNow
            })
        );

        await CreateNotificationAsync(notification, ct);
    }

    public async Task CreateDeadlineReminderNotificationAsync(Guid taskId, CancellationToken ct = default)
    {
        var task = await _context.Tasks.FindAsync(new object[] { taskId }, ct);
        if (task == null || !task.AssignedToId.HasValue) return;

        var notification = new CreateNotificationRequest(
            task.AssignedToId.Value,
            "DeadlineReminder",
            JsonSerializer.Serialize(new
            {
                TaskId = taskId,
                TaskTitle = task.Title,
                DueDate = task.DueDate,
                ReminderSentAt = DateTime.UtcNow
            })
        );

        await CreateNotificationAsync(notification, ct);
    }
}
