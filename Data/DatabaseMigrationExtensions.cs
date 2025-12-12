using Microsoft.EntityFrameworkCore;

namespace ProjectM.Data
{
    public static class DatabaseMigrationExtensions
    {
        public static void ApplyMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            try
            {
                // Apply pending migrations
                context.Database.Migrate();
                Console.WriteLine("Database migrations applied successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying migrations: {ex.Message}");
                throw;
            }
        }

        public static void EnsureDatabaseCreated(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            try
            {
                // Create database if it doesn't exist
                context.Database.EnsureCreated();
                Console.WriteLine("Database ensured created.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ensuring database: {ex.Message}");
                throw;
            }
        }
    }
}
