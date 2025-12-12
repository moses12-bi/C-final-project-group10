using Microsoft.AspNetCore.SignalR;
using ProjectM.Data;

namespace ProjectM.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public NotificationHub(ApplicationDbContext _context)
        {
            _context = _context;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst("sub")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst("sub")?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendNotification(string userId, string message, string type)
        {
            await Clients.Group(userId).SendAsync("ReceiveNotification", new
            {
                message,
                type,
                timestamp = DateTime.UtcNow
            });
        }
    }
}
