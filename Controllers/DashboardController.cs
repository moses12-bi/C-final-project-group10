using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectM.Data;
using ProjectM.DTOs;
using ProjectM.Models;

namespace ProjectM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<DashboardSummaryResponse>> GetSummary()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            // Stats for the current user
            var totalProjects = await _context.ProjectTeammembers
                .CountAsync(pm => pm.UserId == userId);

            var activeTasks = await _context.TaskAssignments
                .Include(ta => ta.Task)
                .CountAsync(ta => ta.UserId == userId && ta.Task!.Status != ProjectM.Models.TaskStatus.Done);

            var completedTasks = await _context.TaskAssignments
                .Include(ta => ta.Task)
                .CountAsync(ta => ta.UserId == userId && ta.Task!.Status == ProjectM.Models.TaskStatus.Done);

            // If admin, show pending invitations count
            var pendingInvitations = 0;
            // Simplified check: if user has any permissions, we assume they can see invites for now 
            // In real app, check specific permission "invites.manage"
            pendingInvitations = await _context.Invitations.CountAsync(i => i.Status == InvitationStatus.Pending);

            var recentProjects = await _context.ProjectTeammembers
                .Where(pm => pm.UserId == userId)
                .Include(pm => pm.Project)
                .OrderByDescending(pm => pm.JoinedAt)
                .Take(5)
                .Select(pm => pm.Project)
                .ToListAsync();

            return Ok(new DashboardSummaryResponse
            {
                TotalProjects = totalProjects,
                ActiveTasks = activeTasks,
                CompletedTasks = completedTasks,
                PendingInvitations = pendingInvitations,
                RecentProjects = recentProjects!
            });
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return userId;
            }
            return null;
        }
    }
}
