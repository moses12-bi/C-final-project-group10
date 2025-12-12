using ProjectM.Models;

namespace ProjectM.Data.Repositories
{
    public interface IProjectTaskRepository : IRepository<ProjectTask>
    {
        Task<ProjectTask?> GetByIdsAsync(int projectId, int taskId);
    }
}



