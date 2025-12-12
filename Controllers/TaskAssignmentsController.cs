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
    [Route("api/tasks/{taskId:int}/assignments")]
    [Authorize]
    public class TaskAssignmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IProjectTaskRepository _tasks;
        private readonly IUserRepository _users;

        public TaskAssignmentsController(ApplicationDbContext context, IProjectTaskRepository tasks, IUserRepository users)
        {
            _context = context;
            _tasks = tasks;
            _users = users;
        }

        [HttpGet]
        [RequirePermission("tasks.read")]
        public async Task<ActionResult<IEnumerable<TaskAssignmentResponseDto>>> GetAssignments(int taskId)
        {
            var task = await _context.ProjectTasks.FindAsync(taskId);
            if (task == null)
            {
                return NotFound(new { message = "Task not found" });
            }

            var assignments = await _context.TaskAssignments
                .Include(a => a.User)
                .Where(a => a.TaskId == taskId)
                .Select(a => new TaskAssignmentResponseDto
                {
                    TaskId = a.TaskId,
                    UserId = a.UserId,
                    UserFullName = a.User.FullName,
                    UserEmail = a.User.Email,
                    IsPrimaryAssignee = a.IsPrimaryAssignee,
                    AssignedAt = DateTime.UtcNow // You might want to add this field to the model
                })
                .ToListAsync();

            return Ok(assignments);
        }

        [HttpPost]
        [RequirePermission("tasks.write")]
        public async Task<ActionResult<TaskAssignmentResponseDto>> AssignUser(int taskId, [FromBody] AssignUserToTaskDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verify task exists
            var task = await _context.ProjectTasks.FindAsync(taskId);
            if (task == null)
            {
                return NotFound(new { message = "Task not found" });
            }

            // Verify user exists
            var user = await _users.GetByIdAsync(dto.UserId);
            if (user == null)
            {
                return BadRequest(new { message = "User not found" });
            }

            // Check if already assigned
            var existingAssignment = await _context.TaskAssignments
                .FirstOrDefaultAsync(a => a.TaskId == taskId && a.UserId == dto.UserId);

            if (existingAssignment != null)
            {
                return BadRequest(new { message = "User is already assigned to this task" });
            }

            // If this is primary assignee, unset other primary assignees
            if (dto.IsPrimaryAssignee)
            {
                var otherPrimaries = await _context.TaskAssignments
                    .Where(a => a.TaskId == taskId && a.IsPrimaryAssignee)
                    .ToListAsync();

                foreach (var assignment in otherPrimaries)
                {
                    assignment.IsPrimaryAssignee = false;
                }
            }

            var newAssignment = new TaskAssignment
            {
                TaskId = taskId,
                UserId = dto.UserId,
                IsPrimaryAssignee = dto.IsPrimaryAssignee
            };

            _context.TaskAssignments.Add(newAssignment);
            await _context.SaveChangesAsync();

            var response = new TaskAssignmentResponseDto
            {
                TaskId = taskId,
                UserId = user.Id,
                UserFullName = user.FullName,
                UserEmail = user.Email,
                IsPrimaryAssignee = dto.IsPrimaryAssignee,
                AssignedAt = DateTime.UtcNow
            };

            return CreatedAtAction(nameof(GetAssignments), new { taskId }, response);
        }

        [HttpDelete("{userId:guid}")]
        [RequirePermission("tasks.write")]
        public async Task<IActionResult> UnassignUser(int taskId, Guid userId)
        {
            var assignment = await _context.TaskAssignments
                .FirstOrDefaultAsync(a => a.TaskId == taskId && a.UserId == userId);

            if (assignment == null)
            {
                return NotFound(new { message = "Assignment not found" });
            }

            _context.TaskAssignments.Remove(assignment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
