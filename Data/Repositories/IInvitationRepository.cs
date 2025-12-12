using ProjectM.Models;
using System.Linq.Expressions;

namespace ProjectM.Data.Repositories
{
    public interface IInvitationRepository : IRepository<Invitation>
    {
        Task<Invitation?> GetByTokenAsync(Guid token);
    }
}
