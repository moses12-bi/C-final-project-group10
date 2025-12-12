using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ProjectM.Attributes;
using ProjectM.Data;
using ProjectM.Data.Repositories;
using ProjectM.DTOs;
using ProjectM.Models;
using ProjectM.Services;

namespace ProjectM.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    [RequirePermission("users.manage")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _users;
        private readonly IInvitationRepository _invitations;
        private readonly IUserPermissionRepository _permissions;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IPermissionService _permissionService;
        private readonly ApplicationDbContext _context;

        public UsersController(
            IUserRepository users,
            IInvitationRepository invitations,
            IUserPermissionRepository permissions,
            IPasswordHasher passwordHasher,
            IPermissionService permissionService,
            ApplicationDbContext context)
        {
            _users = users;
            _invitations = invitations;
            _permissions = permissions;
            _passwordHasher = passwordHasher;
            _permissionService = permissionService;
            _context = context;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminUserDto>>> GetAll()
        {
            var users = await _users.GetAllAsync(
                u => u.UserSkills,
                u => u.ProjectTeammembers,
                u => u.UserPermissions);

            var result = new List<AdminUserDto>();
            foreach (var user in users)
            {
                var permissions = await _permissionService.GetUserPermissionsAsync(user.Id);
                result.Add(new AdminUserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role,
                    Department = user.Department,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    Permissions = permissions
                });
            }

            return Ok(result);
        }

        // GET: api/users/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AdminUserDto>> Get(Guid id)
        {
            var user = await _users.GetByIdAsync(id,
                u => u.UserSkills,
                u => u.ProjectTeammembers,
                u => u.UserPermissions);

            if (user == null)
                return NotFound();

            var permissions = await _permissionService.GetUserPermissionsAsync(user.Id);
            return Ok(new AdminUserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                Department = user.Department,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                Permissions = permissions
            });
        }

        // POST: api/users/invite
        [HttpPost("invite")]
        [RequirePermission("invites.manage")]
        public async Task<IActionResult> InviteUser(InviteUserRequest request)
        {
            // Check email already exists
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return BadRequest("User already exists.");

            var invite = new Invitation
            {
                InvitationId = Guid.NewGuid(),
                Email = request.Email,
                Role = request.Role,
                Department = request.Department,
                PermissionsJson = JsonSerializer.Serialize(request.Permissions),
                Status = InvitationStatus.Pending,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                InvitedByUserId = request.InvitedByUserId
            };

            await _invitations.AddAsync(invite);
            await _invitations.SaveChangesAsync();

            // Email sending handled elsewhere
            return Ok(new
            {
                message = "Invitation created",
                token = invite.InvitationId
            });
        }

        // POST: api/users/complete-registration
        [HttpPost("complete-registration")]
        public async Task<IActionResult> CompleteRegistration(CompleteRegistrationRequest request)
        {
            var invite = await _invitations.GetByTokenAsync(request.Token);

            if (invite == null ||
                invite.Status != InvitationStatus.Pending ||
                invite.ExpiresAt < DateTime.UtcNow)
            {
                return Unauthorized("Invalid or expired invite.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = invite.Email,
                FullName = request.FullName,
                Department = invite.Department,
                Role = invite.Role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                PasswordHash = _passwordHasher.HashPassword(request.Password)
            };

            await _users.AddAsync(user);
            await _users.SaveChangesAsync();

            // Assign permissions from invite
            var permissions = JsonSerializer.Deserialize<Dictionary<string, bool>>(invite.PermissionsJson);

            if (permissions != null)
            {
                foreach (var perm in permissions)
                {
                    await _permissions.AddAsync(new UserPermission
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        PermissionKey = perm.Key,
                        IsGranted = perm.Value
                    });
                }
            }

            await _permissions.SaveChangesAsync();

            invite.Status = InvitationStatus.Used;
            await _invitations.UpdateAsync(invite);
            await _invitations.SaveChangesAsync();

            return Ok("Registration completed.");
        }

        // PUT: api/users/{id}/role
        [HttpPut("{id:guid}/role")]
        public async Task<IActionResult> ChangeRole(Guid id, ChangeRoleRequest request)
        {
            var user = await _users.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            user.Role = request.Role;
            await _users.UpdateAsync(user);
            await _users.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/users/{id}/permissions
        [HttpGet("{id:guid}/permissions")]
        public async Task<ActionResult<Dictionary<string, bool>>> GetPermissions(Guid id)
        {
            var user = await _users.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            var permissions = await _permissionService.GetUserPermissionsAsync(id);
            return Ok(permissions);
        }

        // PUT: api/users/{id}/permissions
        [HttpPut("{id:guid}/permissions")]
        public async Task<IActionResult> UpdatePermissions(Guid id, Dictionary<string, bool> permissions)
        {
            var user = await _users.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            // Validate permission codes exist
            var validCodes = await _context.Permissions
                .AsNoTracking()
                .Select(p => p.Code)
                .ToListAsync();

            foreach (var key in permissions.Keys)
            {
                if (!validCodes.Contains(key))
                {
                    return BadRequest($"Unknown permission code: {key}");
                }
            }

            await _permissions.DeleteByUserIdAsync(id);

            foreach (var perm in permissions)
            {
                await _permissions.AddAsync(new UserPermission
                {
                    Id = Guid.NewGuid(),
                    UserId = id,
                    PermissionKey = perm.Key,
                    IsGranted = perm.Value
                });
            }

            await _permissions.SaveChangesAsync();
            return NoContent();
        }

        // PUT: api/users/{id}/status
        [HttpPut("{id:guid}/status")]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var user = await _users.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            user.IsActive = !user.IsActive;
            await _users.UpdateAsync(user);
            await _users.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _users.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            await _users.DeleteAsync(user);
            await _users.SaveChangesAsync();

            return NoContent();
        }
    }
}
