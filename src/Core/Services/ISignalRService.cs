namespace Core.Services;

public interface ISignalRService
{
    Task SendToUserAsync(string userId, string method, object data);
    Task SendToGroupAsync(string groupName, string method, object data);
    Task SendToAllAsync(string method, object data);
}

public interface INotificationHubContext
{
    Task SendNotificationToUserAsync(string userId, object notification);
    Task SendNotificationToGroupAsync(string groupName, object notification);
    Task SendTaskUpdateAsync(string groupName, object taskUpdate);
    Task SendProjectUpdateAsync(string groupName, object projectUpdate);
}
