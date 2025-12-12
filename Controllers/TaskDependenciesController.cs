using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectM.Attributes;
using ProjectM.Data;

namespace ProjectM.Controllers
{
    [ApiController]
    [Route("api/tasks/{taskId:int}/dependencies")]
    [Authorize]
    public class TaskDependenciesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TaskDependenciesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [RequirePermission("tasks.read")]
        public async Task<ActionResult> GetDependencies(int taskId)
        {
            var task = await _context.ProjectTasks
                .Include(t => t.DependentOnTasks)
                .Include(t => t.DependentTasks)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                blockedBy = task.DependentOnTasks.Select(d => new
                {
                    d.DependsOnTaskId,
                    d.DependsOnTask.Title
                }),
                blocking = task.DependentTasks.Select(d => new
                {
                    d.TaskId,
                    d.Task.Title
                })
            });
        }

        [HttpPost("{dependsOnTaskId:int}")]
        [RequirePermission("tasks.write")]
        public async Task<IActionResult> AddDependency(int taskId, int dependsOnTaskId)
        {
            if (taskId == dependsOnTaskId)
            {
                return BadRequest(new { message = "Task cannot depend on itself" });
            }

            var task = await _context.ProjectTasks.FindAsync(taskId);
            var dependsOnTask = await _context.ProjectTasks.FindAsync(dependsOnTaskId);

            if (task == null || dependsOnTask == null)
            {
                return NotFound(new { message = "Task not found" });
            }

            var existing = await _context.TaskDependencies
                .AnyAsync(d => d.TaskId == taskId && d.DependsOnTaskId == dependsOnTaskId);

            if (existing)
            {
                return BadRequest(new { message = "Dependency already exists" });
            }

            _context.TaskDependencies.Add(new Models.TaskDependency
            {
                TaskId = taskId,
                DependsOnTaskId = dependsOnTaskId
            });

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{dependsOnTaskId:int}")]
        [RequirePermission("tasks.write")]
        public async Task<IActionResult> RemoveDependency(int taskId, int dependsOnTaskId)
        {
            var dependency = await _context.TaskDependencies
                .FirstOrDefaultAsync(d => d.TaskId == taskId && d.DependsOnTaskId == dependsOnTaskId);

            if (dependency == null)
            {
                return NotFound();
            }

            _context.TaskDependencies.Remove(dependency);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
