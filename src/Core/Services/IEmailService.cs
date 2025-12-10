namespace Core.Services;

public interface IEmailService
{
    Task SendInvitationEmailAsync(string email, Guid invitationToken);
    Task SendPasswordResetEmailAsync(string email, string resetToken);
}
