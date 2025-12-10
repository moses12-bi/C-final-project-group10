using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Core.Models;
using Core.Enums;

namespace Infrastructure.Data;

public class AppDbContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSets for domain entities
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectMember> ProjectMembers { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<TaskUpdate> TaskUpdates { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Recommendation> Recommendations { get; set; }
    public DbSet<UserPermission> UserPermissions { get; set; }
    public DbSet<Invitation> Invitations { get; set; }
    public DbSet<UserPermissionEntry> UserPermissionEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure UserProfile
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.Position).HasMaxLength(100);
            entity.Property(e => e.SkillsJson).HasMaxLength(2000);

            entity.Property(e => e.Role)
                  .HasConversion<string>()
                  .HasMaxLength(50);

            entity.Property(e => e.ExperienceLevel)
                  .HasConversion<string>()
                  .HasMaxLength(50);
        });

        // Configure Project
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);

            entity.Property(e => e.Status)
                  .HasConversion<string>()
                  .HasMaxLength(50);
        });

        // Configure ProjectMember
        modelBuilder.Entity<ProjectMember>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Project)
                  .WithMany(p => p.Members)
                  .HasForeignKey(e => e.ProjectId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure TaskItem
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);

            entity.Property(e => e.Status)
                  .HasConversion<string>()
                  .HasMaxLength(50);

            entity.Property(e => e.Priority)
                  .HasConversion<string>()
                  .HasMaxLength(50);

            entity.HasOne(e => e.Project)
                  .WithMany(p => p.Tasks)
                  .HasForeignKey(e => e.ProjectId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.CreatedBy)
                  .WithMany()
                  .HasForeignKey(e => e.CreatedById)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.AssignedTo)
                  .WithMany()
                  .HasForeignKey(e => e.AssignedToId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.DependencyTask)
                  .WithMany()
                  .HasForeignKey(e => e.DependencyTaskId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure TaskUpdate
        modelBuilder.Entity<TaskUpdate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Comment).HasMaxLength(1000);
            entity.Property(e => e.AttachmentUrl).HasMaxLength(500);

            entity.Property(e => e.Status)
                  .HasConversion<string>()
                  .HasMaxLength(50);

            entity.HasOne(e => e.TaskItem)
                  .WithMany(t => t.Updates)
                  .HasForeignKey(e => e.TaskItemId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.UpdatedBy)
                  .WithMany()
                  .HasForeignKey(e => e.UpdatedById)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Notification
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.PayloadJson).HasMaxLength(2000);
        });

        // Configure Recommendation
        modelBuilder.Entity<Recommendation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.RationaleJson).HasMaxLength(2000);

            entity.HasOne(e => e.TaskItem)
                  .WithMany(t => t.Recommendations)
                  .HasForeignKey(e => e.TaskItemId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure UserPermission
        modelBuilder.Entity<UserPermission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.Permission).HasConversion<string>();
            
            // Ensure each user has only one entry per permission
            entity.HasIndex(e => new { e.UserId, e.Permission }).IsUnique();
        });

        // Configure Invitation
        modelBuilder.Entity<Invitation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Department).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
            entity.Property(e => e.PermissionsJson).IsRequired();
        });

        // Configure UserPermissionEntry
        modelBuilder.Entity<UserPermissionEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.PermissionKey).IsRequired().HasMaxLength(100);
            
            // Ensure each user has only one entry per permission key
            entity.HasIndex(e => new { e.UserId, e.PermissionKey }).IsUnique();
        });

        // Seed data for roles
        modelBuilder.Entity<IdentityRole<Guid>>().HasData(
            new IdentityRole<Guid> { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Manager", NormalizedName = "MANAGER" },
            new IdentityRole<Guid> { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "TeamLead", NormalizedName = "TEAMLEAD" },
            new IdentityRole<Guid> { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Employee", NormalizedName = "EMPLOYEE" }
        );
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is TaskItem || e.Entity is TaskUpdate || e.Entity is Notification);

        foreach (var entry in entries)
        {
            if (entry.Entity is TaskItem task)
            {
                if (entry.State == EntityState.Added)
                {
                    task.CreatedAt = DateTime.UtcNow;
                    task.UpdatedAt = DateTime.UtcNow;
                }
                if (entry.State == EntityState.Modified)
                {
                    task.UpdatedAt = DateTime.UtcNow;
                }
            }
            else if (entry.Entity is TaskUpdate update)
            {
                if (entry.State == EntityState.Added)
                {
                    update.CreatedAt = DateTime.UtcNow;
                }
            }
            else if (entry.Entity is Notification notification)
            {
                if (entry.State == EntityState.Added)
                {
                    notification.CreatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}
