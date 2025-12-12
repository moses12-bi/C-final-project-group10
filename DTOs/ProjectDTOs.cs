using System.ComponentModel.DataAnnotations;
using ProjectM.Models;

namespace ProjectM.DTOs
{
    public class CreateProjectDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(500)]
        public string? Goal { get; set; }

        [Required]
        public ProjectStatus Status { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public Guid ManagerId { get; set; }

        public Guid? TeamLeadId { get; set; }
    }

    public class UpdateProjectDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(500)]
        public string? Goal { get; set; }

        [Required]
        public ProjectStatus Status { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public Guid? TeamLeadId { get; set; }
    }

    public class ProjectResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Goal { get; set; }
        public ProjectStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public Guid? TeamLeadId { get; set; }
        public string? TeamLeadName { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TaskCount { get; set; }
        public int TeamMemberCount { get; set; }
    }
}
