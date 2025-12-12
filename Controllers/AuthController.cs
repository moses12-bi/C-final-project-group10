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
                var token = _jwtService.GenerateToken(user, permissions);

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
                var token = _jwtService.GenerateToken(user, permissions);

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
    }

    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
