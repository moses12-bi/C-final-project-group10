using Microsoft.AspNetCore.Mvc;
using ProjectM.Data;
using Microsoft.EntityFrameworkCore;

namespace ProjectM.Controllers
{


[ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TestController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Framework = ".NET 8.0",
                Database = _context.Database.CanConnect() ? "Connected" : "Not Connected"
            });
        }

        [HttpGet("migrate")]
        public async Task<IActionResult> Migrate()
        {
            try
            {
                await _context.Database.MigrateAsync();
                return Ok(new { Message = "Migration completed successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
