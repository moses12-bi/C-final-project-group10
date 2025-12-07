using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.TeamLead;

[Authorize(Roles = "TeamLead")]
public class DashboardModel : PageModel
{
    public void OnGet()
    {
        // Team Lead dashboard logic will be implemented here
        // This will include fetching projects, team status, and recent task updates
    }
}
