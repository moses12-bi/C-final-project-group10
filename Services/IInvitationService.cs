using ProjectM.Models;

namespace ProjectM.Services
{
    public interface IInvitationService
    {
        Task<Invitation> CreateInvitationAsync(string email, string role, string department, List<string> permissions, Guid invitedByUserId);
        Task<Invitation?> ValidateInvitationAsync(string token);
        Task<User?> CompleteRegistrationAsync(string token, string password);
        Task<bool> HasPermissionAsync(Guid userId, string permission);
    }
}
