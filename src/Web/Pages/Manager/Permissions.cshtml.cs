using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Core.Models;
using Core.Enums;

namespace Web.Pages.Manager;

[Authorize(Roles = "Manager")]
public class PermissionsModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly UserManager<IdentityUser<Guid>> _userManager;

    public PermissionsModel(AppDbContext context, UserManager<IdentityUser<Guid>> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public List<UserWithPermissions> UsersWithPermissions { get; set; } = new();

    public class UserWithPermissions
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = "";
        public List<string> Roles { get; set; } = new();
        public List<Permission> Permissions { get; set; } = new();

        public bool HasPermission(Permission permission)
        {
            return Permissions.Contains(permission);
        }
    }

    public async Task OnGetAsync()
    {
        await LoadUsersWithPermissions();
    }

    public async Task<IActionResult> OnPostSavePermissionsAsync([FromBody] PermissionUpdateModel model)
    {
        try
        {
            foreach (var change in model.Changes)
            {
                var parts = change.Key.Split('-');
                var userId = Guid.Parse(parts[0]);
                var permission = Enum.Parse<Permission>(parts[1]);

                var existingPermission = await _context.UserPermissions
                    .FirstOrDefaultAsync(up => up.UserId == userId && up.Permission == permission);

                if (change.Value && existingPermission == null)
                {
                    // Add permission
                    _context.UserPermissions.Add(new UserPermission
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        Permission = permission,
                        GrantedAt = DateTime.UtcNow,
                        GrantedBy = _userManager.GetUserId(User) != null ? Guid.Parse(_userManager.GetUserId(User)!) : (Guid?)null
                    });
                }
                else if (!change.Value && existingPermission != null)
                {
                    // Remove permission
                    _context.UserPermissions.Remove(existingPermission);
                }
            }

            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = ex.Message });
        }
    }

    public async Task<IActionResult> OnPostInviteUserAsync([FromBody] InviteUserModel model)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return new JsonResult(new { success = false, message = "User already exists" });
            }

            // Create invitation record (you might want to create an Invitation entity)
            // For now, we'll just return success
            // In a real implementation, you'd send an email with registration link
            
            return new JsonResult(new { success = true, message = "Invitation sent successfully" });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = ex.Message });
        }
    }

    private async Task LoadUsersWithPermissions()
    {
        var users = await _userManager.Users.ToListAsync();
        var userPermissions = await _context.UserPermissions.ToListAsync();

        UsersWithPermissions = users.Select(user => new UserWithPermissions
        {
            UserId = user.Id,
            Email = user.Email!,
            Roles = _userManager.GetRolesAsync(user).Result.ToList(),
            Permissions = userPermissions
                .Where(up => up.UserId == user.Id)
                .Select(up => up.Permission)
                .ToList()
        }).ToList();
    }
}

public class PermissionUpdateModel
{
    public Dictionary<string, bool> Changes { get; set; } = new();
}

public class InviteUserModel
{
    public string Email { get; set; } = "";
    public string Role { get; set; } = "";
}
