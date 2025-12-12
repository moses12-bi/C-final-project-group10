using Microsoft.EntityFrameworkCore;
using ProjectM.Models;

namespace ProjectM.Data.Repositories
{
    public class InvitationRepository : Repository<Invitation>, IInvitationRepository
    {
        public InvitationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Invitation?> GetByTokenAsync(Guid token)
        {
            return await _context.Set<Invitation>().FirstOrDefaultAsync(i => i.InvitationId == token);
        }
    }
}
