using Microsoft.AspNetCore.Mvc;
using ProjectM.Models;
using ProjectM.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ProjectM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectRepository _projects;

        public ProjectsController(IProjectRepository projects)
        {
            _projects = projects;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetAll()
        {
            var projects = await _projects.GetAllAsync(
                p => p.Tasks,
                p => p.Summaries);
            return Ok(projects);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Project>> Get(int id)
        {
            var project = await _projects.GetByIdAsync(id,
                p => p.Tasks,
                p => p.ProjectTeammembers);

            if (project == null)
            {
                return NotFound();
            }

            return Ok(project);
        }

        [HttpPost]
        public async Task<ActionResult<Project>> Create(Project project)
        {
            await _projects.AddAsync(project);
            await _projects.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = project.Id }, project);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, Project project)
        {
            if (id != project.Id)
            {
                return BadRequest("Id mismatch.");
            }

            await _projects.UpdateAsync(project);

            try
            {
                await _projects.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _projects.ExistsAsync(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _projects.GetByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            await _projects.DeleteAsync(project);
            await _projects.SaveChangesAsync();
            return NoContent();
        }
    }
}

