namespace Core.Services;

public interface IInvitationService
{
    Task<Invitation> CreateInvitationAsync(string email, string role, string department, Dictionary<string, bool> permissions, Guid invitedBy);
    Task<Invitation?> GetInvitationAsync(Guid token);
    Task<bool> ValidateInvitationAsync(Guid token, string email);
    Task<bool> UseInvitationAsync(Guid token);
    Task<List<Invitation>> GetPendingInvitationsAsync();
    Task<bool> RevokeInvitationAsync(Guid invitationId);
}
