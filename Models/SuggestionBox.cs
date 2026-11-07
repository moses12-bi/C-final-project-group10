namespace ProjectM.Models
{
    public class SuggestionBox
    {

        public int Id { get; set; }
        public SuggestionContext Context { get; set; }
        public string SuggestionType { get; set; } = string.Empty;
        public string SuggestedDataJson { get; set; } = string.Empty;
        public bool WasApplied { get; set; } = false;
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public int GeneratedForProjectId { get; set; }
        public int? GeneratedForTaskId { get; set; }
        public int GeneratedByUserId { get; set; }
        //navigation properties

        public Project? GeneratedForProject { get; set; }
        public ProjectTask? GeneratedForTask { get; set; }   
        public User? GeneratedByUser { get; set; }
    }

    public enum SuggestionContext
    {
        ProjectCreation,
        TaskAssignment,
        WorkloadBalance,
        DeadlineAdjustment,
        ResourceAllocation
        //Project,
        //Task,
        //General
    }
}
