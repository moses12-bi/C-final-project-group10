using ProjectM.Models;

namespace ProjectM.Services
{
    public interface IInvitationService
    {
        Task<Invitation> CreateInvitationAsync(string email, string role, string department, Dictionary<string, bool> permissions, Guid invitedByUserId);
        Task<Invitation?> ValidateInvitationAsync(Guid token);
        Task<User?> CompleteRegistrationAsync(Guid token, string password);
        Task<bool> HasPermissionAsync(Guid userId, string permission);
    }
}
