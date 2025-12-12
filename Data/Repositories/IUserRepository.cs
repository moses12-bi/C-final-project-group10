using ProjectM.Models;
using System.Linq.Expressions;

namespace ProjectM.Data.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByIdAsync(Guid id, params Expression<Func<User, object>>[] includes);
    }
}



