using ProjectM.Models;

namespace ProjectM.Data.Repositories
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<bool> ExistsAsync(int id);
    }
}



