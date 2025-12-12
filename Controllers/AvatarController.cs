using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectM.Attributes;
using ProjectM.Data;

namespace ProjectM.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class AvatarController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _uploadPath;

        public AvatarController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _uploadPath = Path.Combine(env.ContentRootPath, "uploads", "avatars");
            Directory.CreateDirectory(_uploadPath);
        }

        [HttpPost("me/avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No file uploaded" });
            }

            // Validate file size (2MB max)
            if (file.Length > 2 * 1024 * 1024)
            {
                return BadRequest(new { message = "File size must be less than 2MB" });
            }

            // Validate file type
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
            {
                return BadRequest(new { message = "Only JPEG, PNG, and GIF images are allowed" });
            }

            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _context.Users.FindAsync(Guid.Parse(userId));
            if (user == null)
            {
                return NotFound();
            }

            // Delete old avatar if exists
            if (!string.IsNullOrEmpty(user.AvatarUrl))
            {
                var oldPath = Path.Combine(_uploadPath, Path.GetFileName(user.AvatarUrl));
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }

            // Save new avatar
            var fileName = $"{userId}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(_uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            user.AvatarUrl = $"/uploads/avatars/{fileName}";
            await _context.SaveChangesAsync();

            return Ok(new { avatarUrl = user.AvatarUrl });
        }

        [HttpDelete("me/avatar")]
        public async Task<IActionResult> DeleteAvatar()
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _context.Users.FindAsync(Guid.Parse(userId));
            if (user == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(user.AvatarUrl))
            {
                var filePath = Path.Combine(_uploadPath, Path.GetFileName(user.AvatarUrl));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                user.AvatarUrl = null;
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}
