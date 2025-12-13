using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectM.Attributes;
using ProjectM.Data;
using System.Security.Claims;

namespace ProjectM.Controllers
{
    [ApiController]
    [Route("api/reports")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("project-status/{projectId:int}")]
        [RequirePermission("analytics.read")]
        public async Task<ActionResult> GetProjectStatusReport(int projectId)
        {
            var project = await _context.Projects
                .Include(p => p.Tasks)
                .Include(p => p.ProjectTeammembers)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
            {
                return NotFound();
            }

            var totalTasks = project.Tasks.Count;
            var completedTasks = project.Tasks.Count(t => t.Status == Models.TaskStatus.Done);
            var inProgressTasks = project.Tasks.Count(t => t.Status == Models.TaskStatus.InProgress);
            var todoTasks = project.Tasks.Count(t => t.Status == Models.TaskStatus.ToDo);

            var totalEstimatedHours = project.Tasks.Sum(t => t.EstimatedHours);
            var totalActualHours = project.Tasks.Sum(t => t.ActualHours ?? 0);

            return Ok(new
            {
                project.Title,
                project.Status,
                project.StartDate,
                project.EndDate,
                TeamSize = project.ProjectTeammembers.Count,
                TotalTasks = totalTasks,
                CompletedTasks = completedTasks,
                InProgressTasks = inProgressTasks,
                TodoTasks = todoTasks,
                CompletionPercentage = totalTasks > 0 ? (completedTasks * 100.0 / totalTasks) : 0,
                TotalEstimatedHours = totalEstimatedHours,
                TotalActualHours = totalActualHours,
                TasksByPriority = project.Tasks.GroupBy(t => t.Priority).Select(g => new
                {
                    Priority = g.Key.ToString(),
                    Count = g.Count()
                })
            });
        }

        [HttpGet("team-performance")]
        [RequirePermission("analytics.read")]
        public async Task<ActionResult> GetTeamPerformanceReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
            var end = endDate ?? DateTime.UtcNow;

            var userPerformance = await _context.TaskAssignments
                .Include(a => a.Task)
                .Include(a => a.User)
                .Where(a => a.Task.CreatedAt >= start && a.Task.CreatedAt <= end)
                .GroupBy(a => new { a.UserId, a.User.FullName })
                .Select(g => new
                {
                    UserId = g.Key.UserId,
                    UserName = g.Key.FullName,
                    TotalTasksAssigned = g.Count(),
                    CompletedTasks = g.Count(a => a.Task.Status == Models.TaskStatus.Done),
                    InProgressTasks = g.Count(a => a.Task.Status == Models.TaskStatus.InProgress),
                    TotalEstimatedHours = g.Sum(a => a.Task.EstimatedHours),
                    TotalActualHours = g.Sum(a => a.Task.ActualHours ?? 0)
                })
                .ToListAsync();

            return Ok(userPerformance);
        }

        [HttpGet("task-analytics")]
        [RequirePermission("analytics.read")]
        public async Task<ActionResult> GetTaskAnalytics([FromQuery] int? projectId)
        {
            var query = _context.ProjectTasks.AsQueryable();

            if (projectId.HasValue)
            {
                query = query.Where(t => t.ProjectId == projectId.Value);
            }

            var tasks = await query.ToListAsync();

            var analytics = new
            {
                TotalTasks = tasks.Count,
                ByStatus = tasks.GroupBy(t => t.Status).Select(g => new
                {
                    Status = g.Key.ToString(),
                    Count = g.Count()
                }),
                ByPriority = tasks.GroupBy(t => t.Priority).Select(g => new
                {
                    Priority = g.Key.ToString(),
                    Count = g.Count()
                }),
                AverageEstimatedHours = tasks.Any() ? tasks.Average(t => (double)t.EstimatedHours) : 0,
                AverageActualHours = tasks.Any(t => t.ActualHours.HasValue) ? tasks.Where(t => t.ActualHours.HasValue).Average(t => (double)t.ActualHours.Value) : 0,
                OverdueTasks = tasks.Count(t => t.Deadline < DateTime.UtcNow && t.Status != Models.TaskStatus.Done),
                CompletionRate = tasks.Count > 0 ? (tasks.Count(t => t.Status == Models.TaskStatus.Done) * 100.0 / tasks.Count) : 0
            };

            return Ok(analytics);
        }
    }
}
