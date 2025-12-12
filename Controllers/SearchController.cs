using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectM.Attributes;
using ProjectM.Data;

namespace ProjectM.Controllers
{
    [ApiController]
    [Route("api/search")]
    [Authorize]
    public class SearchController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GlobalSearch([FromQuery] string query, [FromQuery] string? type = null)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(new { message = "Query parameter is required" });
            }

            var results = new
            {
                Projects = type == null || type == "projects" 
                    ? await SearchProjects(query) 
                    : new List<object>(),
                
                Tasks = type == null || type == "tasks" 
                    ? await SearchTasks(query) 
                    : new List<object>(),
                
                Users = type == null || type == "users" 
                    ? await SearchUsers(query) 
                    : new List<object>()
            };

            return Ok(results);
        }

        private async Task<List<object>> SearchProjects(string query)
        {
            return await _context.Projects
                .Where(p => p.Title.Contains(query) || p.Description.Contains(query))
                .Take(10)
                .Select(p => new
                {
                    Type = "Project",
                    p.Id,
                    p.Title,
                    p.Description,
                    p.Status
                })
                .ToListAsync<object>();
        }

        private async Task<List<object>> SearchTasks(string query)
        {
            return await _context.ProjectTasks
                .Where(t => t.Title.Contains(query) || t.Description.Contains(query))
                .Take(10)
                .Select(t => new
                {
                    Type = "Task",
                    t.Id,
                    t.Title,
                    t.Description,
                    t.Status,
                    t.ProjectId
                })
                .ToListAsync<object>();
        }

        private async Task<List<object>> SearchUsers(string query)
        {
            return await _context.Users
                .Where(u => u.FullName.Contains(query) || u.Email.Contains(query))
                .Take(10)
                .Select(u => new
                {
                    Type = "User",
                    Id = u.Id,
                    u.FullName,
                    u.Email,
                    u.Role,
                    u.Department
                })
                .ToListAsync<object>();
        }
    }
}
