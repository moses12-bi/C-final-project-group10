using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.Account;

public class RegisterModel : PageModel
{
    public void OnGet()
    {
        // This is now an invite-only registration page
        // Users must have a valid invitation token to register
        // The actual registration happens at Register_Invite.cshtml
    }
}
