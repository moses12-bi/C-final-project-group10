using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ProjectM.Data;
using ProjectM.Data.Repositories;
using ProjectM.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new ArgumentNullException("JwtSettings:SecretKey");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"] ?? "ProjectM",
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"] ?? "ProjectMUsers",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Repository services
builder.Services.AddScoped<SeedData>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectTaskRepository, ProjectTaskRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IInvitationRepository, InvitationRepository>();
builder.Services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();

// Authentication and authorization services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IInvitationService, InvitationService>();

// Email service
builder.Services.AddScoped<IEmailService, SmtpEmailService>();

var app = builder.Build();

// Automatically create database and tables (Hibernate-like behavior)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        Console.WriteLine("Initializing Database...");
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        // Detailed feedback on creation
        bool created = context.Database.EnsureCreated(); 
        Console.WriteLine(created ? "Database was created successfully." : "Database already exists (Schema verification skipped).");

        var seeder = services.GetRequiredService<SeedData>();
        seeder.EnsureSeedData();
        Console.WriteLine("Data seeding completed.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"CRITICAL ERROR during Database initialization: {ex.Message}");
        Console.WriteLine(ex.StackTrace);
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating/seeding the database.");
    }
}

// Configure the HTTP request pipeline.
app.UseCors(options =>
    options.WithOrigins("http://localhost:3000")
           .AllowAnyMethod()
           .AllowAnyHeader());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
