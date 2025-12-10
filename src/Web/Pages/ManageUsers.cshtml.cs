using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;

namespace Web.Pages
{
    [Authorize(Roles = "Manager,Admin,HR")]
    public class ManageUsersModel : PageModel
    {
        private readonly SignInManager<IdentityUser<Guid>> _signInManager;
        private readonly UserManager<IdentityUser<Guid>> _userManager;

        public ManageUsersModel(
            SignInManager<IdentityUser<Guid>> signInManager,
            UserManager<IdentityUser<Guid>> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public void OnGet()
        {
            // Manage Users logic would go here
            // For now, this is a placeholder for the UI demonstration
        }
    }
}
