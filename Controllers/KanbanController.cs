using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectM.Attributes;
using ProjectM.Data;

namespace ProjectM.Controllers
{
    [ApiController]
    [Route("api/kanban")]
    [Authorize]
    public class KanbanController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public KanbanController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("project/{projectId:int}")]
        [RequirePermission("tasks.read")]
        public async Task<ActionResult> GetKanbanBoard(int projectId)
        {
            var tasks = await _context.ProjectTasks
                .Include(t => t.Assignments)
                    .ThenInclude(a => a.User)
                .Where(t => t.ProjectId == projectId)
                .Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Description,
                    t.Priority,
                    t.Status,
                    t.EstimatedHours,
                    t.Deadline,
                    Assignees = t.Assignments.Select(a => new
                    {
                        a.UserId,
                        a.User.FullName
                    })
                })
                .ToListAsync();

            var board = new
            {
                ToDo = tasks.Where(t => t.Status == Models.TaskStatus.ToDo).ToList(),
                InProgress = tasks.Where(t => t.Status == Models.TaskStatus.InProgress).ToList(),
                Review = tasks.Where(t => t.Status == Models.TaskStatus.Review).ToList(),
                Done = tasks.Where(t => t.Status == Models.TaskStatus.Done).ToList(),
                Block = tasks.Where(t => t.Status == Models.TaskStatus.Block).ToList()
            };

            return Ok(board);
        }

        [HttpPut("task/{taskId:int}/move")]
        [RequirePermission("tasks.write")]
        public async Task<IActionResult> MoveTask(int taskId, [FromBody] MoveTaskRequest request)
        {
            var task = await _context.ProjectTasks.FindAsync(taskId);
            if (task == null)
            {
                return NotFound(new { message = "Task not found" });
            }

            // Validate status
            if (!Enum.TryParse<Models.TaskStatus>(request.NewStatus, out var newStatus))
            {
                return BadRequest(new { message = "Invalid status" });
            }

            task.Status = newStatus;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class MoveTaskRequest
    {
        public string NewStatus { get; set; } = string.Empty;
    }
}
