using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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
        [RequirePermission("InviteUsers")]
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
                    Permissions = JsonSerializer.Deserialize<List<string>>(invitation.PermissionsJson) ?? new List<string>(),
                    Status = invitation.Status,
                    ExpiresAt = invitation.ExpiresAt,
                    CreatedAt = invitation.CreatedAt,
                    InvitedByUserName = inviter?.FullName ?? "Unknown"
                };

                return CreatedAtAction(nameof(GetInvitation), new { token = invitation.Token }, response);
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

        [HttpGet("{token}")]
        [AllowAnonymous]
        public async Task<ActionResult<InvitationResponse>> GetInvitation(string token)
        {
            try
            {
                var invitation = await _invitationService.ValidateInvitationAsync(token);
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
                    Permissions = JsonSerializer.Deserialize<List<string>>(invitation.PermissionsJson) ?? new List<string>(),
                    Status = invitation.Status,
                    ExpiresAt = invitation.ExpiresAt,
                    CreatedAt = invitation.CreatedAt,
                    InvitedByUserName = inviter?.FullName ?? "Unknown"
                };

                return Ok(response);
            }
            catch (Exception ex)
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
