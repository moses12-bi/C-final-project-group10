using System.ComponentModel.DataAnnotations;
using ProjectM.Models;

namespace ProjectM.DTOs
{
    public class CreateTaskDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Required]
        public TaskPriority Priority { get; set; }

        [Required]
        public ProjectM.Models.TaskStatus Status { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        [Range(0, 10000)]
        public decimal EstimatedHours { get; set; }

        public decimal? ActualHours { get; set; }
    }

    public class UpdateTaskDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Required]
        public TaskPriority Priority { get; set; }

        [Required]
        public ProjectM.Models.TaskStatus Status { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        [Range(0, 10000)]
        public decimal EstimatedHours { get; set; }

        public decimal? ActualHours { get; set; }
    }

    public class TaskResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskPriority Priority { get; set; }
        public ProjectM.Models.TaskStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime Deadline { get; set; }
        public decimal EstimatedHours { get; set; }
        public decimal? ActualHours { get; set; }
        public int ProjectId { get; set; }
        public string? ProjectTitle { get; set; }
        public DateTime CreatedAt { get; set; }
        public int AssigneeCount { get; set; }
        public int CommentCount { get; set; }
        public int AttachmentCount { get; set; }
    }
}
