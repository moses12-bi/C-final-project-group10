using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Core.Models;
using Core.Services;
using System.ComponentModel.DataAnnotations;
using Infrastructure.Data;

namespace Web.Pages.Account;

public class Register_InviteModel : PageModel
{
    private readonly SignInManager<IdentityUser<Guid>> _signInManager;
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IInvitationService _invitationService;
    private readonly AppDbContext _context;

    public Register_InviteModel(
        UserManager<IdentityUser<Guid>> userManager,
        SignInManager<IdentityUser<Guid>> signInManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IInvitationService invitationService,
        AppDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _invitationService = invitationService;
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public Guid Token { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public Invitation? Invitation { get; set; }

    public class InputModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string FullName { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = "";
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (Token == Guid.Empty)
        {
            return Page();
        }

        // Get and validate invitation
        Invitation = await _invitationService.GetInvitationAsync(Token);
        
        if (Invitation == null || !Invitation.IsValid())
        {
            Invitation = null;
            return Page();
        }

        // Pre-fill form with invitation data
        Input.Email = Invitation.Email;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Invitation == null || !Invitation.IsValid())
        {
            ModelState.AddModelError("", "Invalid or expired invitation.");
            return Page();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Verify email matches invitation
        if (Input.Email.ToLowerInvariant() != Invitation.Email.ToLowerInvariant())
        {
            ModelState.AddModelError("", "Email does not match the invitation.");
            return Page();
        }

        try
        {
            // Create user account
            var user = new IdentityUser<Guid> 
            { 
                UserName = Input.Email, 
                Email = Input.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, Input.Password);
            
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            // Create user profile
            var userProfile = new UserProfile
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Email = Input.Email,
                FullName = Input.FullName,
                Department = Invitation.Department,
                Role = Invitation.Role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserProfiles.Add(userProfile);

            // Assign role
            await EnsureRolesExist();
            await _userManager.AddToRoleAsync(user, Invitation.Role);

            // Apply permissions
            foreach (var permission in Invitation.Permissions.Where(p => p.Value))
            {
                var userPermission = new UserPermissionEntry
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    PermissionKey = permission.Key,
                    Value = true,
                    CreatedAt = DateTime.UtcNow,
                    GrantedBy = Invitation.InvitedBy
                };
                _context.UserPermissionEntries.Add(userPermission);
            }

            // Mark invitation as used
            await _invitationService.UseInvitationAsync(Token);

            await _context.SaveChangesAsync();

            // Sign in user
            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToPage("/Manager/Dashboard");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error completing registration: {ex.Message}");
            return Page();
        }
    }

    private async Task EnsureRolesExist()
    {
        var roles = new[] { "Admin", "Supervisor", "User", "Manager", "TeamLead", "Employee" };
        
        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
    }
}
