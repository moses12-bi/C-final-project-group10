using Microsoft.AspNetCore.SignalR;
using Web.Hubs;
using Core.Services;

namespace Web.Services;

public class SignalRService : ISignalRService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendToUserAsync(string userId, string method, object data)
    {
        await _hubContext.Clients.User(userId).SendAsync(method, data);
    }

    public async Task SendToGroupAsync(string groupName, string method, object data)
    {
        await _hubContext.Clients.Group(groupName).SendAsync(method, data);
    }

    public async Task SendToAllAsync(string method, object data)
    {
        await _hubContext.Clients.All.SendAsync(method, data);
    }
}

public class NotificationHubContext : INotificationHubContext
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationHubContext(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNotificationToUserAsync(string userId, object notification)
    {
        await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", notification);
    }

    public async Task SendNotificationToGroupAsync(string groupName, object notification)
    {
        await _hubContext.Clients.Group(groupName).SendAsync("ReceiveNotification", notification);
    }

    public async Task SendTaskUpdateAsync(string groupName, object taskUpdate)
    {
        await _hubContext.Clients.Group(groupName).SendAsync("TaskStatusUpdated", taskUpdate);
    }

    public async Task SendProjectUpdateAsync(string groupName, object projectUpdate)
    {
        await _hubContext.Clients.Group(groupName).SendAsync("ProjectUpdated", projectUpdate);
    }
}
