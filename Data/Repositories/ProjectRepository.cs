using Microsoft.EntityFrameworkCore;
using ProjectM.Models;

namespace ProjectM.Data.Repositories
{
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        public ProjectRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<bool> ExistsAsync(int id)
        {
            return _dbSet.AnyAsync(p => p.Id == id);
        }
    }
}



