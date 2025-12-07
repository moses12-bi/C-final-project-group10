using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.Employee;

[Authorize(Roles = "Employee")]
public class DashboardModel : PageModel
{
    public void OnGet()
    {
        // Employee dashboard logic will be implemented here
        // This will include fetching assigned tasks, performance metrics, and notifications
    }
}
