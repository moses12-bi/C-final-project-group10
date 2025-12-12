using Microsoft.AspNetCore.Mvc;
using ProjectM.Models;
using ProjectM.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ProjectM.Controllers
{
    [ApiController]
    [Route("api/projects/{projectId:int}/[controller]")]
    public class ProjectTasksController : ControllerBase
    {
        private readonly IProjectRepository _projects;
        private readonly IProjectTaskRepository _tasks;

        public ProjectTasksController(IProjectRepository projects, IProjectTaskRepository tasks)
        {
            _projects = projects;
            _tasks = tasks;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectTask>>> GetForProject(int projectId)
        {
            var tasks = await _tasks.GetAllAsync(t => t.Assignments);
            tasks = tasks.Where(t => t.ProjectId == projectId);

            return Ok(tasks);
        }

        [HttpGet("{taskId:int}")]
        public async Task<ActionResult<ProjectTask>> Get(int projectId, int taskId)
        {
            var task = await _tasks.GetByIdsAsync(projectId, taskId);

            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }

        [HttpPost]
        public async Task<ActionResult<ProjectTask>> Create(int projectId, ProjectTask task)
        {
            if (projectId != task.ProjectId)
            {
                task.ProjectId = projectId;
            }

            await _tasks.AddAsync(task);
            await _tasks.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { projectId, taskId = task.Id }, task);
        }

        [HttpPut("{taskId:int}")]
        public async Task<IActionResult> Update(int projectId, int taskId, ProjectTask task)
        {
            if (taskId != task.Id)
            {
                return BadRequest("Id mismatch.");
            }

            task.ProjectId = projectId;
            await _tasks.UpdateAsync(task);

            try
            {
                await _tasks.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _tasks.GetByIdsAsync(projectId, taskId);
                if (exists == null)
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{taskId:int}")]
        public async Task<IActionResult> Delete(int projectId, int taskId)
        {
            var task = await _tasks.GetByIdsAsync(projectId, taskId);
            if (task == null)
            {
                return NotFound();
            }

            await _tasks.DeleteAsync(task);
            await _tasks.SaveChangesAsync();
            return NoContent();
        }
    }
}

