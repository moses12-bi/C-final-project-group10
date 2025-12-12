namespace ProjectM.Models
{
    public class TaskDependency
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int DependsOnTaskId { get; set; }
        public DependencyType Type { get; set; } // Finish-to-Start, Start-to-Start, etc.

        // Navigation properties
        public ProjectTask? Task { get; set; }
        public ProjectTask? DependsOnTask { get; set; }
    }

    public enum DependencyType
    {
        FinishToStart,
        StartToStart,
        FinishToFinish,
        StartToFinish
    }
}
