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

        public async Task<Invitation> CreateInvitationAsync(string email, string role, string department, List<string> permissions, Guid invitedByUserId)
        {
            // Validate inviter permissions
            if (!await HasPermissionAsync(invitedByUserId, "InviteUsers"))
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

            // Generate token
            var token = Guid.NewGuid().ToString("N");

            // Serialize permissions to JSON
            var permissionsJson = JsonSerializer.Serialize(permissions);

            // Create invitation
            var invitation = new Invitation
            {
                InvitationId = Guid.NewGuid(),
                Token = token,
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

            // Send email (async, don't wait for completion)
            _ = Task.Run(() => _emailService.SendInvitationEmailAsync(email, token));

            return invitation;
        }

        public async Task<Invitation?> ValidateInvitationAsync(string token)
        {
            var invitation = await _context.Invitations
                .Include(i => i.InvitedByUser)
                .FirstOrDefaultAsync(i => i.Token == token);

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

        public async Task<User?> CompleteRegistrationAsync(string token, string password)
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
                FullName = invitation.Email.Split('@')[0], // Temporary, will be updated during registration
                PasswordHash = passwordHash,
                Department = invitation.Department,
                Role = invitation.Role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Deserialize permissions and create user permissions
            var permissions = JsonSerializer.Deserialize<List<string>>(invitation.PermissionsJson) ?? new List<string>();
            
            foreach (var permission in permissions)
            {
                var userPermission = new UserPermission
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    PermissionKey = permission,
                    IsGranted = true
                };
                _context.UserPermissions.Add(userPermission);
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
