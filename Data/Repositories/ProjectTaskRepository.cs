using Microsoft.EntityFrameworkCore;
using ProjectM.Models;

namespace ProjectM.Data.Repositories
{
    public class ProjectTaskRepository : Repository<ProjectTask>, IProjectTaskRepository
    {
        public ProjectTaskRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<ProjectTask?> GetByIdsAsync(int projectId, int taskId)
        {
            return _dbSet
                .Include(t => t.Assignments)
                .Include(t => t.Comments)
                .FirstOrDefaultAsync(t => t.ProjectId == projectId && t.Id == taskId);
        }
    }
}



