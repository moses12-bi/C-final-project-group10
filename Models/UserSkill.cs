namespace ProjectM.Models
{
    public class UserSkill
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public SkillLevel Level { get; set; } 

        // Navigation properties
        public User? User { get; set; }
    }

    // Models/Enums/SkillLevel.cs
    public enum SkillLevel
    {
        Beginner,
        Intermediate,
        Expert
    }
}
