namespace ProjectM.Services
{
    public interface IPermissionService
    {
        Task<bool> HasPermissionAsync(Guid userId, string permission);
        Task<List<string>> GetUserPermissionsAsync(Guid userId);
    }
}
