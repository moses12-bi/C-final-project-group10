namespace ProjectM.Models
{
    public class ProjectSummary
    {

        public int Id { get; set; }
        public int ProjectId { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public decimal CompletedPercentage { get; set; }
        public int OverdueTasks { get; set; }
        public decimal BudgetUtilization { get; set; }
        public string RiskLevel { get; set; } = string.Empty;
        public string SummaryJson { get; set; } = string.Empty;

        //naviigation properties
        public Project? Project { get; set; }
    }
}
