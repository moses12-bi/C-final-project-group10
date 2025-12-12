using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectM.Attributes;
using ProjectM.Data;

namespace ProjectM.Controllers
{
    public partial class TaskCommentsController
    {
        [HttpPut("{commentId:int}")]
        [RequirePermission("tasks.write")]
        public async Task<IActionResult> UpdateComment(int taskId, int commentId, [FromBody] UpdateCommentDto dto)
        {
            var comment = await _context.TaskComments
                .FirstOrDefaultAsync(c => c.Id == commentId && c.TaskId == taskId);

            if (comment == null)
            {
                return NotFound(new { message = "Comment not found" });
            }

            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId) || comment.UserId.ToString() != userId)
            {
                return Forbid();
            }

            comment.Content = dto.Content;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{commentId:int}")]
        [RequirePermission("tasks.write")]
        public async Task<IActionResult> DeleteComment(int taskId, int commentId)
        {
            var comment = await _context.TaskComments
                .FirstOrDefaultAsync(c => c.Id == commentId && c.TaskId == taskId);

            if (comment == null)
            {
                return NotFound(new { message = "Comment not found" });
            }

            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId) || comment.UserId.ToString() != userId)
            {
                return Forbid();
            }

            _context.TaskComments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
