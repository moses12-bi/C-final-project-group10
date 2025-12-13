using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectM.Attributes;
using ProjectM.Data;
using ProjectM.Data.Repositories;
using ProjectM.DTOs;
using ProjectM.Models;
using System.Security.Claims;

namespace ProjectM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectRepository _projects;
        private readonly IUserRepository _users;
        private readonly ApplicationDbContext _context;

        public ProjectsController(IProjectRepository projects, IUserRepository users, ApplicationDbContext context)
        {
            _projects = projects;
            _users = users;
            _context = context;
        }

        [HttpGet]
        [RequirePermission("projects.read")]
        public async Task<ActionResult<IEnumerable<Project>>> GetAll()
        {
            var projects = await _projects.GetAllAsync(
                p => p.Tasks,
                p => p.ProjectTeammembers);
            return Ok(projects);
        }

        [HttpGet("{id:int}")]
        [RequirePermission("projects.read")]
        public async Task<ActionResult<Project>> Get(int id)
        {
            var project = await _projects.GetByIdAsync(id,
                p => p.Tasks,
                p => p.ProjectTeammembers);

            if (project == null)
            {
                return NotFound(new { message = "Project not found" });
            }

            return Ok(project);
        }

        [HttpPost]
        [RequirePermission("projects.write")]
        public async Task<ActionResult<Project>> Create([FromBody] CreateProjectDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get current user ID
            var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException());

            // Use provided ManagerId or default to current user
            var managerId = dto.ManagerId ?? currentUserId;

            // Validate manager exists
            var manager = await _users.GetByIdAsync(managerId);
            if (manager == null)
            {
                return BadRequest(new { message = "Manager not found" });
            }

            // Validate team lead if provided
            if (dto.TeamLeadId.HasValue)
            {
                var teamLead = await _users.GetByIdAsync(dto.TeamLeadId.Value);
                if (teamLead == null)
                {
                    return BadRequest(new { message = "Team lead not found" });
                }
            }

            // Validate dates
            if (dto.EndDate < dto.StartDate)
            {
                return BadRequest(new { message = "End date must be after start date" });
            }

            var project = new Project
            {
                Title = dto.Title,
                Description = dto.Description,
                Goal = dto.Goal,
                Status = dto.Status,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                ManagerId = managerId,
                TeamLeadId = dto.TeamLeadId,
                CreatedAt = DateTime.UtcNow
            };

            await _projects.AddAsync(project);
            await _projects.SaveChangesAsync();
            
            return CreatedAtAction(nameof(Get), new { id = project.Id }, project);
        }

        [HttpPut("{id:int}")]
        [RequirePermission("projects.write")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var project = await _projects.GetByIdAsync(id);
            if (project == null)
            {
                return NotFound(new { message = "Project not found" });
            }

            // Validate team lead if provided
            if (dto.TeamLeadId.HasValue)
            {
                var teamLead = await _users.GetByIdAsync(dto.TeamLeadId.Value);
                if (teamLead == null)
                {
                    return BadRequest(new { message = "Team lead not found" });
                }
            }

            // Validate dates
            if (dto.EndDate < dto.StartDate)
            {
                return BadRequest(new { message = "End date must be after start date" });
            }

            // Update fields
            project.Title = dto.Title;
            project.Description = dto.Description;
            project.Goal = dto.Goal;
            project.Status = dto.Status;
            project.StartDate = dto.StartDate;
            project.EndDate = dto.EndDate;
            project.TeamLeadId = dto.TeamLeadId;

            await _projects.UpdateAsync(project);

            try
            {
                await _projects.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _projects.ExistsAsync(id))
                {
                    return NotFound(new { message = "Project not found" });
                }
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [RequirePermission("projects.write")]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _projects.GetByIdAsync(id);
            if (project == null)
            {
                return NotFound(new { message = "Project not found" });
            }

            // Check if project has tasks
            var taskCount = await _context.ProjectTasks.CountAsync(t => t.ProjectId == id);
            if (taskCount > 0)
            {
                return BadRequest(new { message = $"Cannot delete project with {taskCount} existing tasks. Delete tasks first." });
            }

            await _projects.DeleteAsync(project);
            await _projects.SaveChangesAsync();
            
            return NoContent();
        }
    }
}
