using Core.Services;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendInvitationEmailAsync(string email, Guid invitationToken)
    {
        try
        {
            var baseUrl = _configuration["App:BaseUrl"] ?? "https://localhost:7263";
            var registerUrl = $"{baseUrl}/Account/Register?token={invitationToken}";
            
            // For development, just log the URL
            _logger.LogInformation("Invitation URL for {Email}: {Url}", email, registerUrl);
            
            // TODO: Implement actual email sending (SendGrid, SMTP, etc.)
            // await SendEmailAsync(email, "You're Invited!", $"Register here: {registerUrl}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send invitation email to {Email}", email);
            throw;
        }
    }

    public async Task SendPasswordResetEmailAsync(string email, string resetToken)
    {
        try
        {
            var baseUrl = _configuration["App:BaseUrl"] ?? "https://localhost:7263";
            var resetUrl = $"{baseUrl}/Account/ResetPassword?token={resetToken}";
            
            _logger.LogInformation("Password reset URL for {Email}: {Url}", email, resetUrl);
            
            // TODO: Implement actual email sending
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email to {Email}", email);
            throw;
        }
    }
}
