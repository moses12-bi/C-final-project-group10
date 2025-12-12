using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectM.Attributes;
using ProjectM.Data;

namespace ProjectM.Controllers
{
    [ApiController]
    [Route("api/tasks/{taskId:int}/audit")]
    [Authorize]
    public class TaskAuditController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TaskAuditController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [RequirePermission("tasks.read")]
        public async Task<ActionResult> GetAuditLog(int taskId)
        {
            var task = await _context.ProjectTasks.FindAsync(taskId);
            if (task == null)
            {
                return NotFound();
            }

            var logs = await _context.TaskAuditLogs
                .Where(l => l.TaskId == taskId)
                .OrderByDescending(l => l.CreatedAt)
                .Select(l => new
                {
                    l.Id,
                    l.Action,
                    l.Changes,
                    l.UserId,
                    UserName = l.User.FullName,
                    l.CreatedAt
                })
                .ToListAsync();

            return Ok(logs);
        }
    }
}
