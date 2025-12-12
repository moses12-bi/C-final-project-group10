using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectM.Attributes;
using ProjectM.Data;

namespace ProjectM.Controllers
{
    [ApiController]
    [Route("api/calendar")]
    [Authorize]
    public class CalendarController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CalendarController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("tasks")]
        [RequirePermission("tasks.read")]
        public async Task<ActionResult> GetTasksByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] int? projectId = null)
        {
            var query = _context.ProjectTasks
                .Include(t => t.Assignments)
                    .ThenInclude(a => a.User)
                .Where(t => t.Deadline >= startDate && t.Deadline <= endDate);

            if (projectId.HasValue)
            {
                query = query.Where(t => t.ProjectId == projectId.Value);
            }

            var tasks = await query
                .Select(t => new
                {
                    t.Id,
                    t.ProjectId,
                    t.Title,
                    t.Description,
                    t.Priority,
                    t.Status,
                    t.StartDate,
                    t.Deadline,
                    t.EstimatedHours,
                    Assignees = t.Assignments.Select(a => new
                    {
                        a.UserId,
                        a.User.FullName
                    })
                })
                .ToListAsync();

            return Ok(tasks);
        }
    }
}
