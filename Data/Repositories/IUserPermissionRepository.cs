using ProjectM.Models;

namespace ProjectM.Data.Repositories
{
    public interface IUserPermissionRepository : IRepository<UserPermission>
    {
        Task DeleteByUserIdAsync(Guid userId);
    }
}
