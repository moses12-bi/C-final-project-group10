namespace ProjectM.Services
{
    public interface IEmailService
    {
        Task SendInvitationEmailAsync(string email, string token);
    }
}
