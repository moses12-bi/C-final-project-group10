using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models;

public class Invitation
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(255)]
    public string Email { get; set; } = "";
    
    [Required]
    [StringLength(50)]
    public string Role { get; set; } = "";
    
    [Required]
    [StringLength(100)]
    public string Department { get; set; } = "";
    
    [Required]
    public string PermissionsJson { get; set; } = "{}";
    
    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Pending"; // Pending, Used, Expired
    
    [Required]
    public DateTime ExpiresAt { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UsedAt { get; set; }
    
    public Guid? InvitedBy { get; set; }
    
    // Navigation properties
    [ForeignKey("InvitedBy")]
    public virtual IdentityUser? InvitedByUser { get; set; }
    
    // Helper property for permissions
    [NotMapped]
    public Dictionary<string, bool> Permissions
    {
        get
        {
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, bool>>(PermissionsJson) ?? new Dictionary<string, bool>();
            }
            catch
            {
                return new Dictionary<string, bool>();
            }
        }
        set
        {
            PermissionsJson = System.Text.Json.JsonSerializer.Serialize(value);
        }
    }
    
    public bool IsValid()
    {
        return Status == "Pending" && DateTime.UtcNow <= ExpiresAt;
    }
}
