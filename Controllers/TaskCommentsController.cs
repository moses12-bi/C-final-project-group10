using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectM.Attributes;
using ProjectM.Data;
using ProjectM.DTOs;
using ProjectM.Models;
using System.Security.Claims;

namespace ProjectM.Controllers
{
    [ApiController]
    [Route("api/tasks/{taskId:int}/comments")]
    [Authorize]
    public class TaskCommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TaskCommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException("User not authenticated"));
        }

        [HttpGet]
        [RequirePermission("tasks.read")]
        public async Task<ActionResult<IEnumerable<CommentResponseDto>>> GetComments(int taskId)
        {
            var task = await _context.ProjectTasks.FindAsync(taskId);
            if (task == null)
            {
                return NotFound(new { message = "Task not found" });
            }

            var comments = await _context.TaskComments
                .Include(c => c.User)
                .Where(c => c.TaskId == taskId)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new CommentResponseDto
                {
                    Id = c.Id,
                    TaskId = c.TaskId,
                    UserId = c.UserId,
                    UserFullName = c.User.FullName,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();

            return Ok(comments);
        }

        [HttpPost]
        [RequirePermission("tasks.write")]
        public async Task<ActionResult<CommentResponseDto>> CreateComment(int taskId, [FromBody] CreateCommentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var task = await _context.ProjectTasks.FindAsync(taskId);
            if (task == null)
            {
                return NotFound(new { message = "Task not found" });
            }

            var userId = GetCurrentUserId();
            var user = await _context.Users.FindAsync(userId);

            var comment = new TaskComment
            {
                TaskId = taskId,
                UserId = userId,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow
            };

            _context.TaskComments.Add(comment);
            await _context.SaveChangesAsync();

            var response = new CommentResponseDto
            {
                Id = comment.Id,
                TaskId = taskId,
                UserId = userId,
                UserFullName = user?.FullName ?? "Unknown",
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt
            };

            return CreatedAtAction(nameof(GetComments), new { taskId }, response);
        }

        [HttpPut("{commentId:int}")]
        [RequirePermission("tasks.write")]
        public async Task<IActionResult> UpdateComment(int taskId, int commentId, [FromBody] UpdateCommentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var comment = await _context.TaskComments.FindAsync(commentId);
            if (comment == null || comment.TaskId != taskId)
            {
                return NotFound(new { message = "Comment not found" });
            }

            var userId = GetCurrentUserId();
            if (comment.UserId != userId)
            {
                return Forbid("You can only edit your own comments");
            }

            comment.Content = dto.Content;
            comment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{commentId:int}")]
        [RequirePermission("tasks.write")]
        public async Task<IActionResult> DeleteComment(int taskId, int commentId)
        {
            var comment = await _context.TaskComments.FindAsync(commentId);
            if (comment == null || comment.TaskId != taskId)
            {
                return NotFound(new { message = "Comment not found" });
            }

            var userId = GetCurrentUserId();
            if (comment.UserId != userId)
            {
                return Forbid("You can only delete your own comments");
            }

            _context.TaskComments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
