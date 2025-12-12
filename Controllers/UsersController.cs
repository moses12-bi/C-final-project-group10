using Microsoft.AspNetCore.Mvc;
using ProjectM.Models;
using ProjectM.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ProjectM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _users;

        public UsersController(IUserRepository users)
        {
            _users = users;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            var users = await _users.GetAllAsync(
                u => u.UserSkills,
                u => u.ProjectTeammembers);
            return Ok(users);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<User>> Get(Guid id)
        {
            var user = await _users.GetByIdAsync(id,
                u => u.UserSkills,
                u => u.ProjectTeammembers);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> Create(User user)
        {
            await _users.AddAsync(user);
            await _users.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest("Id mismatch.");
            }

            await _users.UpdateAsync(user);

            try
            {
                await _users.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await _users.GetByIdAsync(id) == null)
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _users.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await _users.DeleteAsync(user);
            await _users.SaveChangesAsync();
            return NoContent();
        }
    }
}

