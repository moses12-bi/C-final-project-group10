using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models;

public class UserPermission
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    [StringLength(100)]
    public string PermissionKey { get; set; } = "";
    
    [Required]
    public bool Value { get; set; } = false;
    
    [Required]
    public DateTime CreatedAt { get; set; }
    
    public Guid? GrantedBy { get; set; }
    
    // Navigation properties
    [ForeignKey("UserId")]
    public virtual IdentityUser User { get; set; } = null!;
    
    [ForeignKey("GrantedBy")]
    public virtual IdentityUser? GrantedByUser { get; set; } = null!;
}
