using ProjectM.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ProjectM.Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByIdAsync(Guid id, params Expression<Func<User, object>>[] includes)
        {
            IQueryable<User> query = _dbSet;
            query = includes.Aggregate(query, (current, include) => current.Include(include));
            return await query.FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}



