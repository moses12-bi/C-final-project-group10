using ProjectM.Data;
using ProjectM.Models;

namespace ProjectM.Services
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(Guid userId, string type, string title, string message, int? relatedEntityId = null, string? relatedEntityType = null);
        Task NotifyTaskAssigned(int taskId, Guid userId, string taskTitle);
        Task NotifyCommentAdded(int taskId, Guid taskOwnerId, string commenterName, string taskTitle);
    }

    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateNotificationAsync(
            Guid userId, 
            string type, 
            string title, 
            string message, 
            int? relatedEntityId = null, 
            string? relatedEntityType = null)
        {
            var notification = new Notification
            {
                UserId = userId,
                Type = type,
                Title = title,
                Message = message,
                RelatedEntityId = relatedEntityId,
                RelatedEntityType = relatedEntityType,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task NotifyTaskAssigned(int taskId, Guid userId, string taskTitle)
        {
            await CreateNotificationAsync(
                userId,
                "TaskAssigned",
                "New Task Assignment",
                $"You have been assigned to task: {taskTitle}",
                taskId,
                "Task"
            );
        }

        public async Task NotifyCommentAdded(int taskId, Guid taskOwnerId, string commenterName, string taskTitle)
        {
            await CreateNotificationAsync(
                taskOwnerId,
                "CommentAdded",
                "New Comment",
                $"{commenterName} commented on task: {taskTitle}",
                taskId,
                "Task"
            );
        }
    }
}
