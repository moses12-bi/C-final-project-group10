using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Services;
using Core.DTOs;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly ITaskManagementService _taskManagementService;
    private readonly IRecommendationService _recommendationService;

    public ProjectsController(
        ITaskManagementService taskManagementService,
        IRecommendationService recommendationService)
    {
        _taskManagementService = taskManagementService;
        _recommendationService = recommendationService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request)
    {
        try
        {
            // Project creation logic would be implemented here
            // For now, return a placeholder response
            return Ok(new { Id = Guid.NewGuid(), Name = request.Name });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{projectId}/tasks")]
    public async Task<IActionResult> GetProjectTasks(Guid projectId)
    {
        try
        {
            var tasks = await _taskManagementService.GetTasksByProjectAsync(projectId);
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{projectId}/recommendations")]
    public async Task<IActionResult> GetProjectRecommendations(Guid projectId)
    {
        try
        {
            var recommendations = await _recommendationService.GetBestTeamMembersForProjectAsync(projectId);
            return Ok(recommendations);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{projectId}/risk-assessment")]
    public async Task<IActionResult> GetRiskAssessment(Guid projectId)
    {
        try
        {
            var riskAssessment = await _recommendationService.AssessProjectRiskAsync(projectId);
            return Ok(riskAssessment);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{projectId}/workload-impact/{employeeId}")]
    public async Task<IActionResult> GetWorkloadImpact(Guid projectId, Guid employeeId)
    {
        try
        {
            // This would typically calculate workload impact for a specific employee on a project
            var impact = await _recommendationService.AssessWorkloadImpactAsync(employeeId, Guid.NewGuid()); // Placeholder task ID
            return Ok(impact);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
