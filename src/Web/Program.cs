using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Web.Services;
using Core.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add MVC + Razor Pages + SignalR
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Add Identity with UI support
builder.Services.AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI(); // Add UI support

// Configure application cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.LogoutPath = "/Account/Logout";
});

builder.Services.AddSignalR();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Project Management API",
        Version = "v1",
        Description = "Backend API for Project Management System"
    });
});

// Infrastructure DI (DB, Identity, Repo)
builder.Services.AddInfrastructureServices(builder.Configuration);

// Application Services
builder.Services.AddScoped<ISignalRService, SignalRService>();
builder.Services.AddSingleton<INotificationHubContext, NotificationHubContext>();

// Invitation and Email Services
builder.Services.AddScoped<IInvitationService, Infrastructure.Services.InvitationService>();
builder.Services.AddScoped<IEmailService, Infrastructure.Services.EmailService>();

var app = builder.Build();

// DEVELOPMENT: Enable Swagger + Auto Migrate
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Project API v1");
    });

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    try
    {
        db.Database.EnsureCreated();
    }
    catch
    {
        // If database creation fails, try to create a simple database first
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        var connBuilder = new SqlConnectionStringBuilder(connectionString);
        var dbName = connBuilder.InitialCatalog;
        connBuilder.InitialCatalog = "master"; // Connect to master database
        
        using var connection = new SqlConnection(connBuilder.ToString());
        connection.Open();
        
        var command = connection.CreateCommand();
        command.CommandText = $"IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '{dbName}') CREATE DATABASE [{dbName}]";
        command.ExecuteNonQuery();
        
        // Now try to create tables
        db.Database.EnsureCreated();
    }
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Add route conventions for cleaner URLs
app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    
    // Add custom routes for cleaner URLs
    endpoints.MapGet("/projects", async context =>
    {
        context.Response.Redirect("/Manager/Projects");
        await Task.CompletedTask;
    });
    
    endpoints.MapGet("/dashboard", async context =>
    {
        context.Response.Redirect("/Manager/Dashboard");
        await Task.CompletedTask;
    });
    
    endpoints.MapGet("/team", async context =>
    {
        context.Response.Redirect("/Manager/TeamMembers");
        await Task.CompletedTask;
    });
    
    endpoints.MapGet("/tasks", async context =>
    {
        context.Response.Redirect("/Manager/Tasks");
        await Task.CompletedTask;
    });
});

// Add fallback route for authentication
app.MapGet("/", context => 
{
    if (!context.User.Identity?.IsAuthenticated ?? true)
    {
        context.Response.Redirect("/Account/Login");
        return Task.CompletedTask;
    }
    context.Response.Redirect("/Manager/Dashboard");
    return Task.CompletedTask;
});

// SignalR Hub
app.MapHub<Web.Hubs.NotificationHub>("/notificationHub");

app.Run();
