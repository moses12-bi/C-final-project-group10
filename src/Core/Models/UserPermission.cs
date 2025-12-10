using Core.Enums;

namespace Core.Models;

public class UserPermission
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Permission Permission { get; set; }
    public DateTime GrantedAt { get; set; }
    public Guid? GrantedBy { get; set; }
    
    public IdentityUser User { get; set; } = null!;
    public IdentityUser GrantedByUser { get; set; } = null!;
}
