using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.Manager;

[Authorize(Roles = "Manager")]
public class DashboardModel : PageModel
{
    public void OnGet()
    {
        // Dashboard logic will be implemented here
        // This will include fetching project statistics, team performance metrics, etc.
    }
}
