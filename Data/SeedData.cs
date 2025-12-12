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
            // If users already exist, still run a one-time permission key migration (legacy -> canonical).
            if (_context.Users.Any())
            {
                EnsurePermissionKeyMigration();
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

            // Seed initial permissions (permission-based, not role-based enforcement)
            // These should match Permission.Code values.
            var rolePermissions = new Dictionary<string, List<string>>
            {
                ["Manager"] = new List<string>
                {
                    "users.manage",
                    "invites.manage",
                    "projects.read",
                    "projects.write",
                    "tasks.read",
                    "tasks.write",
                    "analytics.read",
                    "analytics.write",
                    "calendar.read",
                    "calendar.write",
                    "notifications.read",
                    "notifications.write",
                    "files.read",
                    "files.write"
                },
                ["TeamLeader"] = new List<string>
                {
                    "projects.read",
                    "projects.write",
                    "tasks.read",
                    "tasks.write",
                    "analytics.read",
                    "calendar.read",
                    "notifications.read",
                    "files.read"
                },
                ["Employee"] = new List<string>
                {
                    "projects.read",
                    "tasks.read",
                    "analytics.read",
                    "calendar.read",
                    "notifications.read",
                    "files.read"
                }
            };

            foreach (var user in new[] { manager, lead, member })
            {
                if (rolePermissions.TryGetValue(user.Role, out var permissions))
                {
                    foreach (var permission in permissions)
                    {
                        _context.UserPermissions.Add(new UserPermission
                        {
                            Id = Guid.NewGuid(),
                            UserId = user.Id,
                            PermissionKey = permission,
                            IsGranted = true
                        });
                    }
                }
            }
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

        private void EnsurePermissionKeyMigration()
        {
            // Maps legacy permission keys used earlier in the project to the canonical Permission.Code values.
            var legacyMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["InviteUsers"] = "invites.manage",
                ["ManageUsers"] = "users.manage",
                ["ViewProjects"] = "projects.read",
                ["CreateProjects"] = "projects.write",
                ["EditProjects"] = "projects.write",
                ["DeleteProjects"] = "projects.write",
                ["AssignTasks"] = "tasks.write",
                ["ViewAnalytics"] = "analytics.read"
            };

            var legacy = _context.UserPermissions
                .Where(up => legacyMap.Keys.Contains(up.PermissionKey))
                .ToList();

            if (legacy.Count == 0)
            {
                return;
            }

            foreach (var up in legacy)
            {
                var newKey = legacyMap[up.PermissionKey];
                var alreadyExists = _context.UserPermissions.Any(x => x.UserId == up.UserId && x.PermissionKey == newKey);
                if (!alreadyExists)
                {
                    _context.UserPermissions.Add(new UserPermission
                    {
                        Id = Guid.NewGuid(),
                        UserId = up.UserId,
                        PermissionKey = newKey,
                        IsGranted = up.IsGranted
                    });
                }
            }

            _context.SaveChanges();
        }
    }
}
