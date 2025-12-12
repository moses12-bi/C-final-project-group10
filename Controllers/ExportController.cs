using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectM.Attributes;
using ProjectM.Data;
using System.ComponentModel.DataAnnotations;

namespace ProjectM.Controllers
{
    [ApiController]
    [Route("api/export")]
    [Authorize]
    public class ExportController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExportController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("project/{projectId:int}/pdf")]
        [RequirePermission("analytics.read")]
        public async Task<IActionResult> ExportProjectToPdf(int projectId)
        {
            var project = await _context.Projects
                .Include(p => p.Tasks)
                .Include(p => p.ProjectTeammembers)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
            {
                return NotFound();
            }

            // Simple CSV export as PDF generation requires additional library
            var csv = GenerateProjectCsv(project);
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            
            return File(bytes, "text/csv", $"project-{projectId}-report.csv");
        }

        [HttpGet("project/{projectId:int}/excel")]
        [RequirePermission("analytics.read")]
        public async Task<IActionResult> ExportProjectToExcel(int projectId)
        {
            var project = await _context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
            {
                return NotFound();
            }

            var csv = GenerateProjectCsv(project);
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            
            return File(bytes, "text/csv", $"project-{projectId}-export.csv");
        }

        [HttpGet("tasks/excel")]
        [RequirePermission("tasks.read")]
        public async Task<IActionResult> ExportTasksToExcel([FromQuery] int? projectId = null)
        {
            var query = _context.ProjectTasks.AsQueryable();
            
            if (projectId.HasValue)
            {
                query = query.Where(t => t.ProjectId == projectId.Value);
            }

            var tasks = await query.ToListAsync();
            var csv = GenerateTasksCsv(tasks);
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            
            return File(bytes, "text/csv", "tasks-export.csv");
        }

        private string GenerateProjectCsv(Models.Project project)
        {
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Project Report");
            csv.AppendLine($"Title,{project.Title}");
            csv.AppendLine($"Status,{project.Status}");
            csv.AppendLine($"Start Date,{project.StartDate:yyyy-MM-dd}");
            csv.AppendLine($"End Date,{project.EndDate:yyyy-MM-dd}");
            csv.AppendLine();
            csv.AppendLine("Tasks");
            csv.AppendLine("ID,Title,Status,Priority,Deadline,Estimated Hours");
            
            foreach (var task in project.Tasks)
            {
                csv.AppendLine($"{task.Id},{task.Title},{task.Status},{task.Priority},{task.Deadline:yyyy-MM-dd},{task.EstimatedHours}");
            }

            return csv.ToString();
        }

        private string GenerateTasksCsv(List<Models.ProjectTask> tasks)
        {
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("ID,Project ID,Title,Description,Status,Priority,Start Date,Deadline,Estimated Hours,Actual Hours");
            
            foreach (var task in tasks)
            {
                csv.AppendLine($"{task.Id},{task.ProjectId},\"{task.Title}\",\"{task.Description}\",{task.Status},{task.Priority},{task.StartDate:yyyy-MM-dd},{task.Deadline:yyyy-MM-dd},{task.EstimatedHours},{task.ActualHours}");
            }

            return csv.ToString();
        }
    }
}
