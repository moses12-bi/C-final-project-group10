using ProjectM.Models;

namespace ProjectM.Data
{
    public class SeedData
    {
        private readonly ApplicationDbContext _context;

        public SeedData(ApplicationDbContext context)
        {
            _context = context;
        }

        public void EnsureSeedData()
        {
            if (_context.Users.Any())
            {
                return;
            }

            var manager = new User
            {
                FullName = "Jane Manager",
                Email = "jane.manager@example.com",
                PasswordHash = "changeme",
                Role = UserRole.Manager,
                CreatedAt = DateTime.UtcNow
            };

            var lead = new User
            {
                FullName = "Leo Lead",
                Email = "leo.lead@example.com",
                PasswordHash = "changeme",
                Role = UserRole.TeamLeader,
                CreatedAt = DateTime.UtcNow
            };

            var member = new User
            {
                FullName = "Emma Employee",
                Email = "emma.employee@example.com",
                PasswordHash = "changeme",
                Role = UserRole.Employee,
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
