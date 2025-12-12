using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ProjectM.Data;
using ProjectM.Models;

namespace ProjectM.Services
{
    public class InvitationService : IInvitationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;

        public InvitationService(
            ApplicationDbContext context,
            IPasswordHasher passwordHasher,
            IJwtService jwtService,
            IEmailService emailService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _emailService = emailService;
        }

        public async Task<Invitation> CreateInvitationAsync(string email, string role, string department, Dictionary<string, bool> permissions, Guid invitedByUserId)
        {
            // Validate inviter permissions
            if (!await HasPermissionAsync(invitedByUserId, "invites.manage"))
            {
                throw new UnauthorizedAccessException("User does not have permission to create invitations.");
            }

            // Check if user already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("A user with this email already exists.");
            }

            // Check for existing pending invitation
            var existingInvitation = await _context.Invitations
                .FirstOrDefaultAsync(i => i.Email == email && i.Status == InvitationStatus.Pending);
            
            if (existingInvitation != null)
            {
                throw new InvalidOperationException("A pending invitation for this email already exists.");
            }

            // Serialize permissions to JSON
            var permissionsJson = JsonSerializer.Serialize(permissions);

            // Create invitation
            var invitation = new Invitation
            {
                InvitationId = Guid.NewGuid(),
                // Token field kept for backward compat if needed, but we use ID now 
                Token = Guid.NewGuid().ToString("N"),
                Email = email,
                Role = role,
                Department = department,
                PermissionsJson = permissionsJson,
                Status = InvitationStatus.Pending,
                ExpiresAt = DateTime.UtcNow.AddDays(7), // Default 7 days
                InvitedByUserId = invitedByUserId
            };

            _context.Invitations.Add(invitation);
            await _context.SaveChangesAsync();

            // Send email using InvitationId as token
            _ = Task.Run(() => _emailService.SendInvitationEmailAsync(email, invitation.InvitationId.ToString()));

            return invitation;
        }

        public async Task<Invitation?> ValidateInvitationAsync(Guid token)
        {
            var invitation = await _context.Invitations
                .Include(i => i.InvitedByUser)
                .FirstOrDefaultAsync(i => i.InvitationId == token);

            if (invitation == null)
            {
                return null;
            }

            if (invitation.Status != InvitationStatus.Pending)
            {
                return null;
            }

            if (DateTime.UtcNow > invitation.ExpiresAt)
            {
                invitation.Status = InvitationStatus.Expired;
                await _context.SaveChangesAsync();
                return null;
            }

            return invitation;
        }

        public async Task<User?> CompleteRegistrationAsync(Guid token, string password)
        {
            var invitation = await ValidateInvitationAsync(token);
            if (invitation == null)
            {
                return null;
            }

            // Check if user already exists (double-check)
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == invitation.Email);
            if (existingUser != null)
            {
                return null;
            }

            // Hash password
            var passwordHash = _passwordHasher.HashPassword(password);

            // Create user
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = invitation.Email,
                FullName = invitation.Email.Split('@')[0], 
                PasswordHash = passwordHash,
                Department = invitation.Department,
                Role = invitation.Role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Deserialize permissions and create user permissions
            var permissions = JsonSerializer.Deserialize<Dictionary<string, bool>>(invitation.PermissionsJson);

            // Map legacy keys -> canonical permission codes (defensive)
            var legacyMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["InviteUsers"] = "invites.manage",
                ["ManageUsers"] = "users.manage",
                ["ViewProjects"] = "projects.read",
                ["CreateProjects"] = "projects.write",
                ["EditProjects"] = "projects.write",
                ["DeleteProjects"] = "projects.write",
                ["AssignTasks"] = "tasks.write",
                ["ViewAnalytics"] = "analytics.read"
            };

            if (permissions != null)
            {
                foreach (var permission in permissions)
                {
                    var key = legacyMap.TryGetValue(permission.Key, out var mapped) ? mapped : permission.Key;
                    var userPermission = new UserPermission
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        PermissionKey = key,
                        IsGranted = permission.Value
                    };
                    _context.UserPermissions.Add(userPermission);
                }
            }

            // Mark invitation as used
            invitation.Status = InvitationStatus.Used;
            
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<bool> HasPermissionAsync(Guid userId, string permission)
        {
            return await _context.UserPermissions
                .AnyAsync(up => up.UserId == userId && 
                               up.PermissionKey == permission && 
                               up.IsGranted);
        }
    }
}
