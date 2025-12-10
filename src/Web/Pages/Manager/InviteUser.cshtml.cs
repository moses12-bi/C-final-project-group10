using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Core.Models;
using Core.Services;
using System.ComponentModel.DataAnnotations;

namespace Web.Pages.Manager;

[Authorize(Roles = "Manager")]
public class InviteUserModel : PageModel
{
    private readonly IInvitationService _invitationService;
    private readonly UserManager<IdentityUser<Guid>> _userManager;

    public InviteUserModel(IInvitationService invitationService, UserManager<IdentityUser<Guid>> userManager)
    {
        _invitationService = invitationService;
        _userManager = userManager;
    }

    [BindProperty]
    public Invitation Invitation { get; set; } = new();

    public List<Invitation> RecentInvitations { get; set; } = new();

    public class InvitationInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string Department { get; set; } = "";

        [Required]
        public string Role { get; set; } = "";
    }

    public async Task OnGetAsync()
    {
        await LoadRecentInvitations();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadRecentInvitations();
            return Page();
        }

        try
        {
            // Get current user ID
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                ModelState.AddModelError("", "Current user not found.");
                return Page();
            }

            // Parse permissions from form
            var permissions = ParsePermissionsFromForm();

            // Create invitation
            var invitation = await _invitationService.CreateInvitationAsync(
                Invitation.Email,
                Invitation.Role,
                Invitation.Department,
                permissions,
                currentUser.Id
            );

            TempData["Success"] = $"Invitation sent to {Invitation.Email}";
            return RedirectToPage("/Manager/Permissions");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error creating invitation: {ex.Message}");
            await LoadRecentInvitations();
            return Page();
        }
    }

    private async Task LoadRecentInvitations()
    {
        RecentInvitations = await _invitationService.GetPendingInvitationsAsync();
    }

    private Dictionary<string, bool> ParsePermissionsFromForm()
    {
        var permissions = new Dictionary<string, bool>();
        
        // Get all permission checkboxes from form
        var form = Request.Form;
        foreach (var key in form.Keys)
        {
            if (key.StartsWith("permissions[") && key.EndsWith("]"))
            {
                var permissionKey = key.Substring(12, key.Length - 13); // Remove "permissions[" and "]"
                var value = form[key];
                permissions[permissionKey] = value == "true";
            }
        }

        return permissions;
    }
}
