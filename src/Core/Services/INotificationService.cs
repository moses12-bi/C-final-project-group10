using Core.Models;
using Core.DTOs;

namespace Core.Services;

public interface INotificationService
{
    Task<Notification> CreateNotificationAsync(CreateNotificationRequest request, CancellationToken ct = default);
    Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId, CancellationToken ct = default);
    Task MarkNotificationAsReadAsync(Guid notificationId, CancellationToken ct = default);
    Task MarkAllNotificationsAsReadAsync(Guid userId, CancellationToken ct = default);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct = default);
    Task SendRealTimeNotificationAsync(Guid userId, string message, string type = "general", CancellationToken ct = default);
    Task SendProjectNotificationAsync(Guid projectId, string message, string type = "project", CancellationToken ct = default);
    Task SendTaskNotificationAsync(Guid taskId, string message, string type = "task", CancellationToken ct = default);
}

public record NotificationMessage(
    string Type,
    string Title,
    string Message,
    Guid? RelatedEntityId = null,
    string? ActionUrl = null
);
