using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.Manager;

[Authorize(Roles = "Manager")]
public class TestSidebarModel : PageModel
{
    public void OnGet()
    {
        // Simple test page for sidebar
    }
}
