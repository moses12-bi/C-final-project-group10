using System.ComponentModel.DataAnnotations;

namespace ProjectM.DTOs
{
    public class AssignUserToTaskDto
    {
        [Required]
        public Guid UserId { get; set; }

        public bool IsPrimaryAssignee { get; set; } = false;
    }

    public class TaskAssignmentResponseDto
    {
        public int TaskId { get; set; }
        public Guid UserId { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public bool IsPrimaryAssignee { get; set; }
        public DateTime AssignedAt { get; set; }
    }

    public class AddTeamMemberDto
    {
        [Required]
        public Guid UserId { get; set; }
    }

    public class UpdateTeamLeadDto
    {
        [Required]
        public Guid TeamLeadId { get; set; }
    }

    public class TeamMemberResponseDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
    }
}
