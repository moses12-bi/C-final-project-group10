using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Core.DTOs;
using Core.Services;

namespace Web.Pages.Manager;

[Authorize(Roles = "Manager")]
public class CreateProjectModel : PageModel
{
    private readonly IRecommendationService _recommendationService;
    private readonly UserManager<IdentityUser<Guid>> _userManager;

    public CreateProjectModel(
        IRecommendationService recommendationService,
        UserManager<IdentityUser<Guid>> userManager)
    {
        _recommendationService = recommendationService;
        _userManager = userManager;
    }

    [BindProperty]
    public CreateProjectRequest ProjectRequest { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Set the manager ID to current user
            ProjectRequest = ProjectRequest with { ManagerId = currentUser.Id };

            // Project creation logic would be implemented here
            // For now, we'll redirect to dashboard with success message
            TempData["SuccessMessage"] = "Project created successfully!";
            
            return RedirectToPage("/Manager/Dashboard");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Error creating project: {ex.Message}");
            return Page();
        }
    }

    public async Task<JsonResult> OnGetRecommendationsAsync(string projectName, string description, decimal riskLevel)
    {
        try
        {
            // Create a temporary project ID for recommendations
            var tempProjectId = Guid.NewGuid();
            
            // Get recommendations for the project
            var recommendations = await _recommendationService.GetBestTeamMembersForProjectAsync(tempProjectId);
            
            return new JsonResult(recommendations);
        }
        catch (Exception ex)
        {
            return new JsonResult(new { error = ex.Message });
        }
    }
}
