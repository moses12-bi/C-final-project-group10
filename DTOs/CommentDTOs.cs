using System.ComponentModel.DataAnnotations;

namespace ProjectM.DTOs
{
    public class CreateCommentDto
    {
        [Required]
        [StringLength(2000)]
        public string Content { get; set; } = string.Empty;
    }

    public class UpdateCommentDto
    {
        [Required]
        [StringLength(2000)]
        public string Content { get; set; } = string.Empty;
    }

    public class CommentResponseDto
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public Guid UserId { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
