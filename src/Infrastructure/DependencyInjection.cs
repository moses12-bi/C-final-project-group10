using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Core.Interfaces;
using Core.Services;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {


        // Database configuration
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Repositories
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<ITaskUpdateRepository, TaskUpdateRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IRecommendationRepository, RecommendationRepository>();

        // Services
        services.AddScoped<IRecommendationService, RecommendationService>();
        services.AddScoped<ITaskManagementService, TaskManagementService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IReportingService, ReportingService>();

        return services;
    }
}
