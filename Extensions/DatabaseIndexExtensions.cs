using Microsoft.EntityFrameworkCore;
using ProjectM.Data;

namespace ProjectM.Extensions
{
    public static class DatabaseIndexExtensions
    {
        public static void CreatePerformanceIndexes(this ModelBuilder modelBuilder)
        {
            // Project indexes
            modelBuilder.Entity<Models.Project>()
                .HasIndex(p => p.Status)
                .HasDatabaseName("IX_Projects_Status");

            modelBuilder.Entity<Models.Project>()
                .HasIndex(p => p.StartDate)
                .HasDatabaseName("IX_Projects_StartDate");

            modelBuilder.Entity<Models.Project>()
                .HasIndex(p => p.EndDate)
                .HasDatabaseName("IX_Projects_EndDate");

            // Task indexes
            modelBuilder.Entity<Models.ProjectTask>()
                .HasIndex(t => t.ProjectId)
                .HasDatabaseName("IX_Tasks_ProjectId");

            modelBuilder.Entity<Models.ProjectTask>()
                .HasIndex(t => t.Status)
                .HasDatabaseName("IX_Tasks_Status");

            modelBuilder.Entity<Models.ProjectTask>()
                .HasIndex(t => t.Priority)
                .HasDatabaseName("IX_Tasks_Priority");

            modelBuilder.Entity<Models.ProjectTask>()
                .HasIndex(t => t.Deadline)
                .HasDatabaseName("IX_Tasks_Deadline");

            modelBuilder.Entity<Models.ProjectTask>()
                .HasIndex(t => new { t.ProjectId, t.Status })
                .HasDatabaseName("IX_Tasks_ProjectId_Status");

            // TaskAssignment indexes
            modelBuilder.Entity<Models.TaskAssignment>()
                .HasIndex(ta => ta.TaskId)
                .HasDatabaseName("IX_TaskAssignments_TaskId");

            modelBuilder.Entity<Models.TaskAssignment>()
                .HasIndex(ta => ta.UserId)
                .HasDatabaseName("IX_TaskAssignments_UserId");

            modelBuilder.Entity<Models.TaskAssignment>()
                .HasIndex(ta => new { ta.TaskId, ta.UserId })
                .IsUnique()
                .HasDatabaseName("IX_TaskAssignments_TaskId_UserId");

            // Comment indexes
            modelBuilder.Entity<Models.TaskComment>()
                .HasIndex(c => c.TaskId)
                .HasDatabaseName("IX_Comments_TaskId");

            modelBuilder.Entity<Models.TaskComment>()
                .HasIndex(c => c.CreatedAt)
                .HasDatabaseName("IX_Comments_CreatedAt");

            // Notification indexes
            modelBuilder.Entity<Models.Notification>()
                .HasIndex(n => n.UserId)
                .HasDatabaseName("IX_Notifications_UserId");

            modelBuilder.Entity<Models.Notification>()
                .HasIndex(n => n.IsRead)
                .HasDatabaseName("IX_Notifications_IsRead");

            modelBuilder.Entity<Models.Notification>()
                .HasIndex(n => new { n.UserId, n.IsRead })
                .HasDatabaseName("IX_Notifications_UserId_IsRead");

            // User indexes
            modelBuilder.Entity<Models.User>()
                .HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");

            modelBuilder.Entity<Models.User>()
                .HasIndex(u => u.Role)
                .HasDatabaseName("IX_Users_Role");
        }
    }
}
