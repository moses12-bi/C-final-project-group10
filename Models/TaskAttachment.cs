namespace ProjectM.Models
{
    public class TaskAttachment
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public int TaskId { get; set; }
        public Guid UploadedById { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ProjectTask? Task { get; set; }
        public User? UploadedBy { get; set; }
    }
}
