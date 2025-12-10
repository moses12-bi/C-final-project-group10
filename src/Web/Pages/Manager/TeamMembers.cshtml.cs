using Microsoft.AspNetCore.Mvc.RazorPages;
using Core.Models;
using Core.Enums;

namespace Web.Pages.Manager;

public class TeamMembersModel : PageModel
{
    public List<TeamMemberViewModel> TeamMembers { get; set; } = new();

    public void OnGet()
    {
        // TODO: Replace with actual data from repository
        // For now, return sample data
        TeamMembers = new List<TeamMemberViewModel>
        {
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Role = UserRole.TeamLead,
                ExperienceLevel = ExperienceLevel.Senior,
                CurrentWorkload = 75,
                IsActive = true
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                Role = UserRole.Employee,
                ExperienceLevel = ExperienceLevel.MidLevel,
                CurrentWorkload = 60,
                IsActive = true
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Mike",
                LastName = "Johnson",
                Email = "mike.johnson@example.com",
                Role = UserRole.Employee,
                ExperienceLevel = ExperienceLevel.Junior,
                CurrentWorkload = 40,
                IsActive = true
            }
        };
    }
}

public class TeamMemberViewModel
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public UserRole Role { get; set; }
    public ExperienceLevel ExperienceLevel { get; set; }
    public int CurrentWorkload { get; set; }
    public bool IsActive { get; set; }
}
