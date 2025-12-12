using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectM.Data;

namespace ProjectM.Controllers
{
    [ApiController]
    [Route("api/users/me/skills")]
    [Authorize]
    public class UserSkillsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserSkillsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetSkills()
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var skills = await _context.UserSkills
                .Where(s => s.UserId.ToString() == userId)
                .Select(s => new
                {
                    s.Id,
                    s.SkillName,
                    s.ProficiencyLevel
                })
                .ToListAsync();

            return Ok(skills);
        }

        [HttpPost]
        public async Task<IActionResult> AddSkill([FromBody] AddSkillDto dto)
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var skill = new Models.UserSkill
            {
                UserId = Guid.Parse(userId),
                SkillName = dto.SkillName,
                ProficiencyLevel = dto.ProficiencyLevel
            };

            _context.UserSkills.Add(skill);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                skill.Id,
                skill.SkillName,
                skill.ProficiencyLevel
            });
        }

        [HttpDelete("{skillId:int}")]
        public async Task<IActionResult> RemoveSkill(int skillId)
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var skill = await _context.UserSkills
                .FirstOrDefaultAsync(s => s.Id == skillId && s.UserId.ToString() == userId);

            if (skill == null)
            {
                return NotFound();
            }

            _context.UserSkills.Remove(skill);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class AddSkillDto
    {
        public string SkillName { get; set; } = string.Empty;
        public string ProficiencyLevel { get; set; } = "Beginner";
    }
}
