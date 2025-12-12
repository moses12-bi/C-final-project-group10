using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ProjectM.Services;
using ProjectM.DTOs;
using ProjectM.Models;
using ProjectM.Data;
using ProjectM.Attributes;
using System.Text.Json;

namespace ProjectM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvitationController : ControllerBase
    {
        private readonly IInvitationService _invitationService;
        private readonly IPermissionService _permissionService;
        private readonly ApplicationDbContext _context;

        public InvitationController(
            IInvitationService invitationService,
            IPermissionService permissionService,
            ApplicationDbContext context)
        {
            _invitationService = invitationService;
            _permissionService = permissionService;
            _context = context;
        }

        [HttpPost]
        [Authorize]
        [RequirePermission("invites.manage")]
        public async Task<ActionResult<InvitationResponse>> CreateInvitation([FromBody] CreateInvitationRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }

                var invitation = await _invitationService.CreateInvitationAsync(
                    request.Email,
                    request.Role,
                    request.Department,
                    request.Permissions,
                    userId.Value);

                // Get inviter user name
                var inviter = await _context.Users.FindAsync(userId.Value);

                var response = new InvitationResponse
                {
                    InvitationId = invitation.InvitationId,
                    Email = invitation.Email,
                    Role = invitation.Role,
                    Department = invitation.Department,
                    Permissions = JsonSerializer.Deserialize<Dictionary<string, bool>>(invitation.PermissionsJson) ?? new Dictionary<string, bool>(),
                    Status = invitation.Status,
                    ExpiresAt = invitation.ExpiresAt,
                    CreatedAt = invitation.CreatedAt,
                    InvitedByUserName = inviter?.FullName ?? "Unknown"
                };

                return CreatedAtAction(nameof(GetInvitation), new { token = invitation.InvitationId }, response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the invitation.");
            }
        }

        [HttpGet]
        [Authorize]
        [RequirePermission("invites.manage")]
        public async Task<ActionResult<IEnumerable<InvitationResponse>>> ListPendingInvitations()
        {
            var invitations = await _context.Invitations
                .AsNoTracking()
                .Where(i => i.Status == InvitationStatus.Pending)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            var inviterIds = invitations.Select(i => i.InvitedByUserId).Distinct().ToList();
            var inviters = await _context.Users
                .AsNoTracking()
                .Where(u => inviterIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.FullName);

            var result = invitations.Select(inv => new InvitationResponse
            {
                InvitationId = inv.InvitationId,
                Email = inv.Email,
                Role = inv.Role,
                Department = inv.Department,
                Permissions = JsonSerializer.Deserialize<Dictionary<string, bool>>(inv.PermissionsJson) ?? new Dictionary<string, bool>(),
                Status = inv.Status,
                ExpiresAt = inv.ExpiresAt,
                CreatedAt = inv.CreatedAt,
                InvitedByUserName = inviters.TryGetValue(inv.InvitedByUserId, out var name) ? name : "Unknown"
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{token}")]
        [AllowAnonymous]
        public async Task<ActionResult<InvitationResponse>> GetInvitation(string token)
        {
            try
            {
                if (!Guid.TryParse(token, out Guid tokenGuid))
                {
                    return BadRequest("Invalid token format.");
                }

                var invitation = await _invitationService.ValidateInvitationAsync(tokenGuid);
                if (invitation == null)
                {
                    return NotFound("Invalid or expired invitation token.");
                }

                var inviter = await _context.Users.FindAsync(invitation.InvitedByUserId);

                var response = new InvitationResponse
                {
                    InvitationId = invitation.InvitationId,
                    Email = invitation.Email,
                    Role = invitation.Role,
                    Department = invitation.Department,
                    Permissions = JsonSerializer.Deserialize<Dictionary<string, bool>>(invitation.PermissionsJson) ?? new Dictionary<string, bool>(),
                    Status = invitation.Status,
                    ExpiresAt = invitation.ExpiresAt,
                    CreatedAt = invitation.CreatedAt,
                    InvitedByUserName = inviter?.FullName ?? "Unknown"
                };

                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while validating the invitation.");
            }
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
