using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectM.Attributes;
using ProjectM.Data;
using ProjectM.Data.Repositories;
using ProjectM.DTOs;
using ProjectM.Models;

namespace ProjectM.Controllers
{
    [ApiController]
    [Route("api/projects/{projectId:int}/tasks")]
    [Authorize]
    public class ProjectTasksController : ControllerBase
    {
        private readonly IProjectRepository _projects;
        private readonly IProjectTaskRepository _tasks;
        private readonly ApplicationDbContext _context;

        public ProjectTasksController(IProjectRepository projects, IProjectTaskRepository tasks, ApplicationDbContext context)
        {
            _projects = projects;
            _tasks = tasks;
            _context = context;
        }

        [HttpGet]
        [RequirePermission("tasks.read")]
        public async Task<ActionResult<IEnumerable<ProjectTask>>> GetForProject(int projectId)
        {
            // Verify project exists
            var project = await _projects.GetByIdAsync(projectId);
            if (project == null)
            {
                return NotFound(new { message = "Project not found" });
            }

            var tasks = await _tasks.GetAllAsync(t => t.Assignments);
            tasks = tasks.Where(t => t.ProjectId == projectId);

            return Ok(tasks);
        }

        [HttpGet("{taskId:int}")]
        [RequirePermission("tasks.read")]
        public async Task<ActionResult<ProjectTask>> Get(int projectId, int taskId)
        {
            var task = await _tasks.GetByIdsAsync(projectId, taskId);

            if (task == null)
            {
                return NotFound(new { message = "Task not found" });
            }

            return Ok(task);
        }

        [HttpPost]
        [RequirePermission("tasks.write")]
        public async Task<ActionResult<ProjectTask>> Create(int projectId, [FromBody] CreateTaskDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verify project exists
            var project = await _projects.GetByIdAsync(projectId);
            if (project == null)
            {
                return BadRequest(new { message = "Project not found" });
            }

            // Validate dates
            if (dto.Deadline < dto.StartDate)
            {
                return BadRequest(new { message = "Deadline must be after start date" });
            }

            var task = new ProjectTask
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                Status = dto.Status,
                StartDate = dto.StartDate,
                Deadline = dto.Deadline,
                EstimatedHours = dto.EstimatedHours,
                ActualHours = dto.ActualHours,
                ProjectId = projectId,
                CreatedAt = DateTime.UtcNow
            };

            await _tasks.AddAsync(task);
            await _tasks.SaveChangesAsync();
            
            return CreatedAtAction(nameof(Get), new { projectId, taskId = task.Id }, task);
        }

        [HttpPut("{taskId:int}")]
        [RequirePermission("tasks.write")]
        public async Task<IActionResult> Update(int projectId, int taskId, [FromBody] UpdateTaskDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var task = await _tasks.GetByIdsAsync(projectId, taskId);
            if (task == null)
            {
                return NotFound(new { message = "Task not found" });
            }

            // Validate dates
            if (dto.Deadline < dto.StartDate)
            {
                return BadRequest(new { message = "Deadline must be after start date" });
            }

            // Update fields
            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Priority = dto.Priority;
            task.Status = dto.Status;
            task.StartDate = dto.StartDate;
            task.Deadline = dto.Deadline;
            task.EstimatedHours = dto.EstimatedHours;
            task.ActualHours = dto.ActualHours;

            await _tasks.UpdateAsync(task);

            try
            {
                await _tasks.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _tasks.GetByIdsAsync(projectId, taskId);
                if (exists == null)
                {
                    return NotFound(new { message = "Task not found" });
                }
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{taskId:int}")]
        [RequirePermission("tasks.write")]
        public async Task<IActionResult> Delete(int projectId, int taskId)
        {
            var task = await _tasks.GetByIdsAsync(projectId, taskId);
            if (task == null)
            {
                return NotFound(new { message = "Task not found" });
            }

            // Check for dependencies (comments, assignments, attachments)
            var hasComments = await _context.TaskComments.AnyAsync(c => c.TaskId == taskId);
            var hasAssignments = await _context.TaskAssignments.AnyAsync(a => a.TaskId == taskId);
            var hasAttachments = await _context.TaskAttachments.AnyAsync(a => a.TaskId == taskId);

            if (hasComments || hasAssignments || hasAttachments)
            {
                // Delete related records first
                var comments = _context.TaskComments.Where(c => c.TaskId == taskId);
                var assignments = _context.TaskAssignments.Where(a => a.TaskId == taskId);
                var attachments = _context.TaskAttachments.Where(a => a.TaskId == taskId);

                _context.TaskComments.RemoveRange(comments);
                _context.TaskAssignments.RemoveRange(assignments);
                _context.TaskAttachments.RemoveRange(attachments);
            }

            await _tasks.DeleteAsync(task);
            await _tasks.SaveChangesAsync();
            
            return NoContent();
        }
    }
}
