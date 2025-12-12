using Microsoft.EntityFrameworkCore;
using ProjectM.Models;

namespace ProjectM.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Project> Projects => Set<Project>();
        public DbSet<ProjectTask> ProjectTasks => Set<ProjectTask>();
        public DbSet<ProjectSummary> ProjectSummaries => Set<ProjectSummary>();
        public DbSet<ProjectAuditLog> ProjectAuditLogs => Set<ProjectAuditLog>();
        public DbSet<ProjectTeammember> ProjectTeammembers => Set<ProjectTeammember>();
        public DbSet<TaskAssignment> TaskAssignments => Set<TaskAssignment>();
        public DbSet<TaskDependency> TaskDependencies => Set<TaskDependency>();
        public DbSet<TaskComment> TaskComments => Set<TaskComment>();
        public DbSet<TaskAttachment> TaskAttachments => Set<TaskAttachment>();
        public DbSet<TaskAuditLog> TaskAuditLogs => Set<TaskAuditLog>();
        public DbSet<User> Users => Set<User>();
        public DbSet<UserSkill> UserSkills => Set<UserSkill>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<PerformanceMetric> PerformanceMetrics => Set<PerformanceMetric>();
        public DbSet<SuggestionBox> SuggestionBoxes => Set<SuggestionBox>();

        // New auth/permission related DbSets
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<UserPermission> UserPermissions => Set<UserPermission>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<OtpCode> OtpCodes => Set<OtpCode>();
        public DbSet<Invitation> Invitations => Set<Invitation>();
        public DbSet<AccessRequest> AccessRequests => Set<AccessRequest>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Project>(entity =>
            {
                entity.Property(p => p.Title).IsRequired().HasMaxLength(200);
                entity.Property(p => p.Description).HasMaxLength(2000);
                entity.HasOne(p => p.Manager)
                      .WithMany(u => u.ManagedProjects)
                      .HasForeignKey(p => p.ManagerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.TeamLead)
                      .WithMany()
                      .HasForeignKey(p => p.TeamLeadId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ProjectTask>(entity =>
            {
                entity.Property(t => t.Title).IsRequired().HasMaxLength(200);
                entity.HasOne(t => t.Project)
                      .WithMany(p => p.Tasks)
                      .HasForeignKey(t => t.ProjectId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.ParentTask)
                      .WithMany(t => t.SubTasks)
                      .HasForeignKey(t => t.ParentTaskId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TaskAssignment>(entity =>
            {
                entity.HasOne(a => a.Task)
                      .WithMany(t => t.Assignments)
                      .HasForeignKey(a => a.TaskId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.User)
                      .WithMany(u => u.AssignedTasks)
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TaskDependency>(entity =>
            {
                entity.HasOne(d => d.Task)
                      .WithMany(t => t.Dependencies)
                      .HasForeignKey(d => d.TaskId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.DependsOnTask)
                      .WithMany()
                      .HasForeignKey(d => d.DependsOnTaskId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ProjectTeammember>(entity =>
            {
                entity.HasOne(tm => tm.Project)
                      .WithMany(p => p.ProjectTeammembers)
                      .HasForeignKey(tm => tm.ProjectId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(tm => tm.User)
                      .WithMany(u => u.ProjectTeammembers)
                      .HasForeignKey(tm => tm.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ProjectAuditLog>(entity =>
            {
                entity.HasOne(l => l.Project)
                      .WithMany(p => p.AuditLogs)
                      .HasForeignKey(l => l.ProjectId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(l => l.User)
                      .WithMany()
                      .HasForeignKey(l => l.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ProjectSummary>(entity =>
            {
                entity.HasOne(s => s.Project)
                      .WithMany(p => p.Summaries)
                      .HasForeignKey(s => s.ProjectId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TaskComment>(entity =>
            {
                entity.HasOne(c => c.Task)
                      .WithMany(t => t.Comments)
                      .HasForeignKey(c => c.TaskId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.User)
                      .WithMany()
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TaskAttachment>(entity =>
            {
                entity.HasOne(a => a.Task)
                      .WithMany(t => t.Attachments)
                      .HasForeignKey(a => a.TaskId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.UploadedBy)
                      .WithMany()
                      .HasForeignKey(a => a.UploadedById)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TaskAuditLog>(entity =>
            {
                entity.HasOne(l => l.Task)
                      .WithMany(t => t.AuditLogs)
                      .HasForeignKey(l => l.TaskId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(l => l.User)
                      .WithMany()
                      .HasForeignKey(l => l.UserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<UserSkill>(entity =>
            {
                entity.HasOne(us => us.User)
                      .WithMany(u => u.UserSkills)
                      .HasForeignKey(us => us.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasOne(n => n.User)
                      .WithMany(u => u.Notification)
                      .HasForeignKey(n => n.UserId);
                entity.HasOne(n => n.RelatedTask)
                      .WithMany()
                      .HasForeignKey(n => n.RelatedTaskId)
                      .OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(n => n.RelatedProject)
                      .WithMany()
                      .HasForeignKey(n => n.RelatedProjectId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<SuggestionBox>(entity =>
            {
                entity.HasOne(s => s.GeneratedForProject)
                      .WithMany()
                      .HasForeignKey(s => s.GeneratedForProjectId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.GeneratedForTask)
                      .WithMany()
                      .HasForeignKey(s => s.GeneratedForTaskId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(s => s.GeneratedByUser)
                      .WithMany()
                      .HasForeignKey(s => s.GeneratedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Permissions and auth-related configuration
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasIndex(p => p.Code).IsUnique();
                entity.Property(p => p.Code).HasMaxLength(100);
            });

            modelBuilder.Entity<UserPermission>(entity =>
            {
                entity.HasIndex(up => new { up.UserId, up.PermissionKey }).IsUnique();
                entity.HasOne(up => up.User)
                      .WithMany(u => u.UserPermissions)
                      .HasForeignKey(up => up.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasIndex(rt => rt.Token).IsUnique();
                entity.HasOne(rt => rt.User)
                      .WithMany(u => u.RefreshTokens)
                      .HasForeignKey(rt => rt.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OtpCode>(entity =>
            {
                entity.HasOne(o => o.User)
                      .WithMany(u => u.OtpCodes)
                      .HasForeignKey(o => o.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Invitation>(entity =>
            {
                entity.HasIndex(i => i.Token).IsUnique();
                entity.Property(i => i.Email).HasMaxLength(256);
                entity.HasOne(i => i.InvitedByUser)
                      .WithMany(u => u.InvitationsSent)
                      .HasForeignKey(i => i.InvitedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AccessRequest>(entity =>
            {
                entity.Property(ar => ar.Email).HasMaxLength(256);
                entity.HasOne(ar => ar.ReviewedByUser)
                      .WithMany()
                      .HasForeignKey(ar => ar.ReviewedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Seed initial permission codes (ids fixed for HasData)
            modelBuilder.Entity<Permission>().HasData(new Permission { Id = 1, Code = "users.manage", Description = "Manage users and permissions" },
                                                    new Permission { Id = 2, Code = "invites.manage", Description = "Create and manage invitations" },
                                                    new Permission { Id = 3, Code = "projects.read", Description = "Read projects" },
                                                    new Permission { Id = 4, Code = "projects.write", Description = "Create or modify projects" },
                                                    new Permission { Id = 5, Code = "tasks.read", Description = "Read tasks" },
                                                    new Permission { Id = 6, Code = "tasks.write", Description = "Create or modify tasks" },
                                                    new Permission { Id = 7, Code = "analytics.read", Description = "Read analytics and reports" },
                                                    new Permission { Id = 8, Code = "calendar.read", Description = "Read calendar events" },
                                                    new Permission { Id = 9, Code = "calendar.write", Description = "Write calendar events" },
                                                    new Permission { Id = 10, Code = "notifications.read", Description = "Read notifications" });
        }
    }
}
