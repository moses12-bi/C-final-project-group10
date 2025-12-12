namespace ProjectM.Services
{
    public interface IPermissionService
    {
        Task<bool> HasPermissionAsync(Guid userId, string permission);
        Task<Dictionary<string, bool>> GetUserPermissionsAsync(Guid userId);
    }
}
