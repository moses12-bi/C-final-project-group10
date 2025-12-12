namespace ProjectM.DTOs
{
    public class DashboardSummaryResponse
    {
        public int TotalProjects { get; set; }
        public int ActiveTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingInvitations { get; set; }
        public List<ProjectM.Models.Project> RecentProjects { get; set; } = new();
    }
}
