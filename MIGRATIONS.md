## Database Migration Guide

### Overview
This project uses Entity Framework Core for database management. We provide migration support for production deployments.

### Development Setup

**Option 1: Quick Start (EnsureCreated)**
```bash
# Development mode - automatically creates database
dotnet run
```

**Option 2: Using Migrations (Recommended for Production)**
```bash
# Install EF Core tools
dotnet tool install --global dotnet-ef

# Create initial migration
dotnet ef migrations add InitialCreate

# Apply migrations
dotnet ef database update
```

### Migration Commands

```bash
# Add a new migration
dotnet ef migrations add <MigrationName>

# Apply pending migrations
dotnet ef database update

# Rollback to specific migration
dotnet ef database update <MigrationName>

# Remove last migration (before applying)
dotnet ef migrations remove

# Generate SQL script
dotnet ef migrations script
```

### Production Deployment

1. **Generate SQL Script**
```bash
dotnet ef migrations script --output migration.sql
```

2. **Review and Apply**
   - Review the generated SQL
   - Apply to production database manually or via CI/CD

3. **Connection String**
   - Update `appsettings.Production.json`
   - Ensure proper credentials and security

### Database Seeding

Seed data is configured in `SeedData.cs` and runs automatically on application start.

### Backup Recommendations

- Always backup before migrations
- Test migrations in staging first
- Keep migration scripts in version control
