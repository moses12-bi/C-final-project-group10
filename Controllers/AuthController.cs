using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using ProjectM.Services;
using ProjectM.DTOs;
using ProjectM.Models;
using ProjectM.Data;
using System.Security.Claims;

namespace ProjectM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IInvitationService _invitationService;
        private readonly IJwtService _jwtService;
        private readonly IPermissionService _permissionService;
        private readonly ApplicationDbContext _context;

        public AuthController(
            IInvitationService invitationService,
            IJwtService jwtService,
            IPermissionService permissionService,
            ApplicationDbContext context)
        {
            _invitationService = invitationService;
            _jwtService = jwtService;
            _permissionService = permissionService;
            _context = context;
        }

        [HttpPost("complete-registration")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> CompleteRegistration([FromBody] CompleteRegistrationRequest request)
        {
            try
            {
                var user = await _invitationService.CompleteRegistrationAsync(request.Token, request.Password);
                if (user == null)
                {
                    return BadRequest("Invalid or expired invitation token.");
                }

                // Update user full name
                user.FullName = request.FullName;
                await _context.SaveChangesAsync();

                // Get user permissions
                var permissions = await _permissionService.GetUserPermissionsAsync(user.Id);

                // Generate JWT token
                var grantedPermissions = permissions.Where(p => p.Value).Select(p => p.Key).ToList();
                var token = _jwtService.GenerateToken(user, permissions.Where(p => p.Value).Select(p => p.Key).ToList());

                var response = new AuthResponse
                {
                    Token = token,
                    User = user,
                    Permissions = permissions
                };

                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while completing registration.");
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);

                if (user == null)
                {
                    return Unauthorized("Invalid email or password.");
                }

                var passwordHasher = new PasswordHasher();
                if (!passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
                {
                    return Unauthorized("Invalid email or password.");
                }

                var permissions = await _permissionService.GetUserPermissionsAsync(user.Id);
                var token = _jwtService.GenerateToken(user, permissions.Where(p => p.Value).Select(p => p.Key).ToList());

                var response = new AuthResponse
                {
                    Token = token,
                    User = user,
                    Permissions = permissions
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred during login.");
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserResponse>> GetCurrentUser()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }

                var user = await _context.Users.FindAsync(userId.Value);
                if (user == null)
                {
                    return NotFound();
                }

                var permissions = await _permissionService.GetUserPermissionsAsync(user.Id);

                var response = new UserResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role,
                    Department = user.Department,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    Permissions = permissions
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving user information.");
            }
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return userId;
            }
            return null;
        }
// [POST] Register (Public)
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                {
                    return BadRequest("Email already registered.");
                }

                var passwordHasher = new PasswordHasher();
                var passwordHash = passwordHasher.HashPassword(request.Password);

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = request.Email,
                    FullName = request.FullName,
                    PasswordHash = passwordHash,
                    Role = "Manager", // Default role for self-registration
                    Department = "General", // Default department
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                
                // For the first user or public registration, we might want to give basic permissions
                // Or if it's the "Manager" role, give them invite permissions
                var defaultPermissions = new List<string>
                {
                    "users.manage",
                    "invites.manage",
                    "projects.read",
                    "projects.write",
                    "tasks.read",
                    "tasks.write",
                    "analytics.read",
                    "analytics.write",
                    "calendar.read",
                    "calendar.write",
                    "notifications.read",
                    "notifications.write",
                    "files.read",
                    "files.write"
                };

                foreach (var perm in defaultPermissions)
                {
                    _context.UserPermissions.Add(new UserPermission
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        PermissionKey = perm,
                        IsGranted = true
                    });
                }

                await _context.SaveChangesAsync();

                // Login the user immediately
                var permissions = await _permissionService.GetUserPermissionsAsync(user.Id);
                var token = _jwtService.GenerateToken(user, permissions.Where(p => p.Value).Select(p => p.Key).ToList());

                return Ok(new AuthResponse
                {
                    Token = token,
                    User = user,
                    Permissions = permissions
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registration Error: {ex}");
                return StatusCode(500, $"An error occurred during registration: {ex.Message} {ex.InnerException?.Message}");
            }
        }
    }

    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}
