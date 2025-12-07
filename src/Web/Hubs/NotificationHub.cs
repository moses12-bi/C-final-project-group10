using Microsoft.AspNetCore.SignalR;
using Core.Enums;

namespace Web.Hubs;

public class NotificationHub : Hub
{
    public async Task JoinProjectGroup(string projectId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Project_{projectId}");
    }

    public async Task LeaveProjectGroup(string projectId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Project_{projectId}");
    }

    public async Task JoinUserGroup(string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
    }

    public async Task LeaveUserGroup(string userId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
    }

    public async Task SendNotificationToUser(string userId, string message)
    {
        await Clients.User(userId).SendAsync("ReceiveNotification", message);
    }

    public async Task SendNotificationToGroup(string groupName, string message)
    {
        await Clients.Group(groupName).SendAsync("ReceiveNotification", message);
    }

    // Kanban board real-time updates
    public async Task TaskStatusChanged(string taskId, string newStatus, string projectId)
    {
        await Clients.Group($"Project_{projectId}").SendAsync("TaskStatusUpdated", new
        {
            TaskId = taskId,
            NewStatus = newStatus,
            UpdatedAt = DateTime.UtcNow
        });
    }

    public async Task TaskAssigned(string taskId, string assignedToId, string projectId)
    {
        await Clients.Group($"Project_{projectId}").SendAsync("TaskAssigned", new
        {
            TaskId = taskId,
            AssignedToId = assignedToId,
            UpdatedAt = DateTime.UtcNow
        });
    }

    public async Task TaskCreated(string taskId, string projectId)
    {
        await Clients.Group($"Project_{projectId}").SendAsync("TaskCreated", new
        {
            TaskId = taskId,
            UpdatedAt = DateTime.UtcNow
        });
    }

    public async Task TaskDeleted(string taskId, string projectId)
    {
        await Clients.Group($"Project_{projectId}").SendAsync("TaskDeleted", new
        {
            TaskId = taskId,
            UpdatedAt = DateTime.UtcNow
        });
    }
}
