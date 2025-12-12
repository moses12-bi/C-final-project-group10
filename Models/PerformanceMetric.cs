namespace ProjectM.Models
{
    public class PerformanceMetric
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime MeasuredDate { get; set; }
        public int TasksCompleted { get; set; }
        public int TasksOnTime { get; set; }
        public int TasksDelayed { get; set; }
        public decimal AverageCompletionTimeHours { get; set; }
        public decimal QualityScore { get; set; } // 0-100

        // Navigation properties
        public User? User { get; set; }
    }
}
