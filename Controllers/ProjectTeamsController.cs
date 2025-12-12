using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectM.Attributes;
using ProjectM.Data;
using ProjectM.Data.Repositories;
using ProjectM.DTOs;
using ProjectM.Models;

namespace ProjectM.Controllers
{
    [ApiController]
    [Route("api/projects/{projectId:int}/team")]
    [Authorize]
    public class ProjectTeamsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IProjectRepository _projects;
        private readonly IUserRepository _users;

        public ProjectTeamsController(ApplicationDbContext context, IProjectRepository projects, IUserRepository users)
        {
            _context = context;
            _projects = projects;
            _users = users;
        }

        [HttpGet]
        [RequirePermission("projects.read")]
        public async Task<ActionResult<IEnumerable<TeamMemberResponseDto>>> GetTeamMembers(int projectId)
        {
            var project = await _projects.GetByIdAsync(projectId);
            if (project == null)
            {
                return NotFound(new { message = "Project not found" });
            }

            var teamMembers = await _context.ProjectTeammembers
                .Include(tm => tm.User)
                .Where(tm => tm.ProjectId == projectId)
                .Select(tm => new TeamMemberResponseDto
                {
                    UserId = tm.UserId,
                    FullName = tm.User.FullName,
                    Email = tm.User.Email,
                    Role = tm.User.Role,
                    Department = tm.User.Department ?? string.Empty
                })
                .ToListAsync();

            return Ok(teamMembers);
        }

        [HttpPost]
        [RequirePermission("projects.write")]
        public async Task<ActionResult<TeamMemberResponseDto>> AddTeamMember(int projectId, [FromBody] AddTeamMemberDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verify project exists
            var project = await _projects.GetByIdAsync(projectId);
            if (project == null)
            {
                return NotFound(new { message = "Project not found" });
            }

            // Verify user exists
            var user = await _users.GetByIdAsync(dto.UserId);
            if (user == null)
            {
                return BadRequest(new { message = "User not found" });
            }

            // Check if already a team member
            var existing = await _context.ProjectTeammembers
                .FirstOrDefaultAsync(tm => tm.ProjectId == projectId && tm.UserId == dto.UserId);

            if (existing != null)
            {
                return BadRequest(new { message = "User is already a team member" });
            }

            var teamMember = new ProjectTeammember
            {
                ProjectId = projectId,
                UserId = dto.UserId
            };

            _context.ProjectTeammembers.Add(teamMember);
            await _context.SaveChangesAsync();

            var response = new TeamMemberResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                Department = user.Department ?? string.Empty
            };

            return CreatedAtAction(nameof(GetTeamMembers), new { projectId }, response);
        }

        [HttpDelete("{userId:guid}")]
        [RequirePermission("projects.write")]
        public async Task<IActionResult> RemoveTeamMember(int projectId, Guid userId)
        {
            var teamMember = await _context.ProjectTeammembers
                .FirstOrDefaultAsync(tm => tm.ProjectId == projectId && tm.UserId == userId);

            if (teamMember == null)
            {
                return NotFound(new { message = "Team member not found" });
            }

            _context.ProjectTeammembers.Remove(teamMember);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("lead")]
        [RequirePermission("projects.write")]
        public async Task<IActionResult> UpdateTeamLead(int projectId, [FromBody] UpdateTeamLeadDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var project = await _projects.GetByIdAsync(projectId);
            if (project == null)
            {
                return NotFound(new { message = "Project not found" });
            }

            // Verify user exists
            var user = await _users.GetByIdAsync(dto.TeamLeadId);
            if (user == null)
            {
                return BadRequest(new { message = "User not found" });
            }

            // Update team lead
            project.TeamLeadId = dto.TeamLeadId;
            await _projects.UpdateAsync(project);
            await _projects.SaveChangesAsync();

            return NoContent();
        }
    }
}
