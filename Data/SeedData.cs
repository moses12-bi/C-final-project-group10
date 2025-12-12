using ProjectM.Models;
using ProjectM.Services;

namespace ProjectM.Data
{
    public class SeedData
    {
        private readonly ApplicationDbContext _context;
        private readonly ProjectM.Services.IPasswordHasher _passwordHasher;

        public SeedData(ApplicationDbContext context, ProjectM.Services.IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public void EnsureSeedData()
        {
            if (_context.Users.Any())
            {
                return;
            }

            var manager = new User
            {
                Id = Guid.NewGuid(),
                FullName = "Jane Manager",
                Email = "jane.manager@example.com",
                PasswordHash = _passwordHasher.HashPassword("changeme"),
                Role = "Manager",
                Department = "Management",
                CreatedAt = DateTime.UtcNow
            };

            var lead = new User
            {
                Id = Guid.NewGuid(),
                FullName = "Leo Lead",
                Email = "leo.lead@example.com",
                PasswordHash = _passwordHasher.HashPassword("changeme"),
                Role = "TeamLeader",
                Department = "Development",
                CreatedAt = DateTime.UtcNow
            };

            var member = new User
            {
                Id = Guid.NewGuid(),
                FullName = "Emma Employee",
                Email = "emma.employee@example.com",
                PasswordHash = _passwordHasher.HashPassword("changeme"),
                Role = "Employee",
                Department = "Development",
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.AddRange(manager, lead, member);
            _context.SaveChanges();

            var project = new Project
            {
                Title = "Sample Project",
                Description = "Initial seeded project",
                Goal = "Deliver MVP",
                Status = ProjectStatus.InProgress,
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddMonths(1),
                ManagerId = manager.Id,
                TeamLeadId = lead.Id
            };

            _context.Projects.Add(project);
            _context.SaveChanges();

            var task = new ProjectTask
            {
                Title = "Design API",
                Description = "Draft REST endpoints",
                Priority = TaskPriority.High,
                Status = ProjectM.Models.TaskStatus.InProgress,
                StartDate = DateTime.UtcNow.Date,
                Deadline = DateTime.UtcNow.Date.AddDays(7),
                EstimatedHours = 16,
                ProjectId = project.Id
            };

            _context.ProjectTasks.Add(task);
            _context.SaveChanges();

            _context.TaskAssignments.Add(new TaskAssignment
            {
                TaskId = task.Id,
                UserId = member.Id,
                IsPrimaryAssignee = true
            });

            _context.ProjectTeammembers.AddRange(
                new ProjectTeammember { ProjectId = project.Id, UserId = lead.Id },
                new ProjectTeammember { ProjectId = project.Id, UserId = member.Id }
            );

            _context.SaveChanges();
        }
    }
}
