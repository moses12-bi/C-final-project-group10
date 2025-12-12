using Microsoft.EntityFrameworkCore;
using ProjectM.Data;

namespace ProjectM.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly ApplicationDbContext _context;

        public PermissionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> HasPermissionAsync(Guid userId, string permission)
        {
            return await _context.UserPermissions
                .AnyAsync(up => up.UserId == userId && 
                               up.PermissionKey == permission && 
                               up.IsGranted);
        }

        public async Task<List<string>> GetUserPermissionsAsync(Guid userId)
        {
            return await _context.UserPermissions
                .Where(up => up.UserId == userId && up.IsGranted)
                .Select(up => up.PermissionKey)
                .ToListAsync();
        }
    }
}
