using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Core.Enums;
using Core.Services;
using Core.DTOs;
using Web.Hubs;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskManagementService _taskManagementService;
    private readonly IRecommendationService _recommendationService;
    private readonly IHubContext<NotificationHub> _hubContext;

    public TasksController(
        ITaskManagementService taskManagementService,
        IRecommendationService recommendationService,
        IHubContext<NotificationHub> hubContext)
    {
        _taskManagementService = taskManagementService;
        _recommendationService = recommendationService;
        _hubContext = hubContext;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var task = await _taskManagementService.CreateTaskAsync(request);
            
            // Broadcast task creation via SignalR
            await _hubContext.Clients.Group($"Project_{request.ProjectId}")
                .SendAsync("TaskCreated", new { TaskId = task.Id, UpdatedAt = DateTime.UtcNow });

            return Ok(task);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{taskId}/status")]
    public async Task<IActionResult> UpdateTaskStatus(Guid taskId, [FromBody] UpdateTaskStatusRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var task = await _taskManagementService.UpdateTaskStatusAsync(taskId, request.Status, userId, request.Comment);
            
            // Broadcast status change via SignalR
            await _hubContext.Clients.Group($"Project_{task.ProjectId}")
                .SendAsync("TaskStatusUpdated", new { TaskId = taskId, NewStatus = request.Status, UpdatedAt = DateTime.UtcNow });

            return Ok(task);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{taskId}/assign")]
    public async Task<IActionResult> AssignTask(Guid taskId, [FromBody] AssignTaskRequest request)
    {
        try
        {
            var task = await _taskManagementService.AssignTaskAsync(taskId, request.AssignedToId);
            
            // Broadcast assignment via SignalR
            await _hubContext.Clients.Group($"Project_{task.ProjectId}")
                .SendAsync("TaskAssigned", new { TaskId = taskId, AssignedToId = request.AssignedToId, UpdatedAt = DateTime.UtcNow });

            return Ok(task);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{taskId}/recommendations")]
    public async Task<IActionResult> GetTaskRecommendations(Guid taskId)
    {
        try
        {
            var recommendations = await _recommendationService.GetBestEmployeesForTaskAsync(taskId);
            return Ok(recommendations);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{taskId}/deadline-estimate")]
    public async Task<IActionResult> GetDeadlineEstimate(Guid taskId)
    {
        try
        {
            var estimate = await _recommendationService.GetEstimatedDeadlineAsync(taskId);
            return Ok(estimate);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{taskId}/updates")]
    public async Task<IActionResult> GetTaskUpdates(Guid taskId)
    {
        try
        {
            var updates = await _taskManagementService.GetTaskUpdatesAsync(taskId);
            return Ok(updates);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{taskId}/updates")]
    public async Task<IActionResult> AddTaskUpdate(Guid taskId, [FromBody] CreateTaskUpdateRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var update = await _taskManagementService.AddTaskUpdateAsync(taskId, userId, request.Comment, request.EffortLogged);
            return Ok(update);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("overdue")]
    public async Task<IActionResult> GetOverdueTasks()
    {
        try
        {
            var tasks = await _taskManagementService.GetOverdueTasksAsync();
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("critical")]
    public async Task<IActionResult> GetCriticalTasks()
    {
        try
        {
            var tasks = await _taskManagementService.GetCriticalTasksAsync();
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private Guid GetCurrentUserId()
    {
        // Get current user ID from claims
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;
    }
}

public record UpdateTaskStatusRequest(TaskStatus Status, string? Comment = null);
public record AssignTaskRequest(Guid AssignedToId);
public record CreateTaskUpdateRequest(string Comment, double EffortLogged);
