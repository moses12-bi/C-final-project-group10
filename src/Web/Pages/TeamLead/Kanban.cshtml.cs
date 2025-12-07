using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Web.Hubs;

namespace Web.Pages.TeamLead;

[Authorize(Roles = "TeamLead,Manager")]
public class KanbanModel : PageModel
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public KanbanModel(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public void OnGet()
    {
        // Kanban board logic will be implemented here
        // This page provides the real-time drag-and-drop interface for task management
    }

    public async Task<IActionResult> OnPostUpdateTaskStatusAsync(Guid taskId, string status)
    {
        // Update task status and broadcast via SignalR
        await _hubContext.Clients.Group($"Project_{GetProjectIdFromTask(taskId)}")
            .SendAsync("TaskStatusUpdated", new { TaskId = taskId, NewStatus = status, UpdatedAt = DateTime.UtcNow });

        return new JsonResult(new { success = true });
    }

    private Guid GetProjectIdFromTask(Guid taskId)
    {
        // Implementation to get project ID from task
        // This would typically involve a database lookup
        return Guid.NewGuid(); // Placeholder
    }
}
