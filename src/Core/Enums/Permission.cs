namespace Core.Enums;

public enum Permission
{
    // Project Management
    ViewProjects,
    CreateProjects,
    EditProjects,
    DeleteProjects,
    
    // Task Management
    ViewTasks,
    CreateTasks,
    EditTasks,
    DeleteTasks,
    AssignTasks,
    
    // Team Management
    ViewTeamMembers,
    InviteUsers,
    ManagePermissions,
    EditTeamMembers,
    
    // Reports & Analytics
    ViewReports,
    ExportReports,
    ViewAnalytics,
    
    // System Administration
    ManageSettings,
    ViewSystemLogs,
    ManageRoles,
    
    // Basic Access
    ViewDashboard,
    EditOwnProfile,
    ViewOwnTasks
}
