using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectM.Attributes;
using ProjectM.Data;
using ProjectM.Models;
using System.Security.Claims;

namespace ProjectM.Controllers
{
    [ApiController]
    [Route("api/tasks/{taskId:int}/attachments")]
    [Authorize]
    public class TaskAttachmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _uploadPath;

        public TaskAttachmentsController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "task-attachments");
            
            // Ensure upload directory exists
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException("User not authenticated"));
        }

        [HttpGet]
        [RequirePermission("tasks.read")]
        public async Task<ActionResult> GetAttachments(int taskId)
        {
            var task = await _context.ProjectTasks.FindAsync(taskId);
            if (task == null)
            {
                return NotFound(new { message = "Task not found" });
            }

            var attachments = await _context.TaskAttachments
                .Include(a => a.User)
                .Where(a => a.TaskId == taskId)
                .Select(a => new
                {
                    a.Id,
                    a.TaskId,
                    a.FileName,
                    a.FileSize,
                    a.ContentType,
                    a.UploadedAt,
                    UploadedBy = a.User.FullName
                })
                .ToListAsync();

            return Ok(attachments);
        }

        [HttpPost]
        [RequirePermission("tasks.write")]
        public async Task<IActionResult> UploadAttachment(int taskId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No file provided" });
            }

            var task = await _context.ProjectTasks.FindAsync(taskId);
            if (task == null)
            {
                return NotFound(new { message = "Task not found" });
            }

            // Validate file size (10MB max)
            if (file.Length > 10 * 1024 * 1024)
            {
                return BadRequest(new { message = "File size exceeds 10MB limit" });
            }

            // Generate unique filename
            var fileExtension = Path.GetExtension(file.FileName);
            var storedFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(_uploadPath, storedFileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var userId = GetCurrentUserId();
            var attachment = new TaskAttachment
            {
                TaskId = taskId,
                FileName = file.FileName,
                StoredFileName = storedFileName,
                FilePath = filePath,
                FileSize = file.Length,
                ContentType = file.ContentType,
                UploadedBy = userId,
                UploadedAt = DateTime.UtcNow
            };

            _context.TaskAttachments.Add(attachment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAttachments), new { taskId }, new
            {
                attachment.Id,
                attachment.FileName,
                attachment.FileSize,
                attachment.ContentType,
                attachment.UploadedAt
            });
        }

        [HttpGet("{attachmentId:int}/download")]
        [RequirePermission("tasks.read")]
        public async Task<IActionResult> DownloadAttachment(int taskId, int attachmentId)
        {
            var attachment = await _context.TaskAttachments
                .FirstOrDefaultAsync(a => a.Id == attachmentId && a.TaskId == taskId);

            if (attachment == null)
            {
                return NotFound(new { message = "Attachment not found" });
            }

            if (!System.IO.File.Exists(attachment.FilePath))
            {
                return NotFound(new { message = "File not found on server" });
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(attachment.FilePath);
            return File(bytes, attachment.ContentType, attachment.FileName);
        }

        [HttpDelete("{attachmentId:int}")]
        [RequirePermission("tasks.write")]
        public async Task<IActionResult> DeleteAttachment(int taskId, int attachmentId)
        {
            var attachment = await _context.TaskAttachments
                .FirstOrDefaultAsync(a => a.Id == attachmentId && a.TaskId == taskId);

            if (attachment == null)
            {
                return NotFound(new { message = "Attachment not found" });
            }

            // Delete physical file
            if (System.IO.File.Exists(attachment.FilePath))
            {
                System.IO.File.Delete(attachment.FilePath);
            }

            _context.TaskAttachments.Remove(attachment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
