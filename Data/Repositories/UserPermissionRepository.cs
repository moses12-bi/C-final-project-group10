using Microsoft.EntityFrameworkCore;
using ProjectM.Models;

namespace ProjectM.Data.Repositories
{
    public class UserPermissionRepository : Repository<UserPermission>, IUserPermissionRepository
    {
        public UserPermissionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task DeleteByUserIdAsync(Guid userId)
        {
            var permissions = await _context.Set<UserPermission>()
                .Where(p => p.UserId == userId)
                .ToListAsync();

            if (permissions.Any())
            {
                _context.Set<UserPermission>().RemoveRange(permissions);
            }
        }
    }
}
