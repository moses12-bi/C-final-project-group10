namespace ProjectM.Models
{
    public class ProjectTeammember
    {

        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Guid UserId { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LeftAt { get; set; }

        //navigation properties
        public Project? Project { get; set; }
        public User? User { get; set; }

    }

    
}
