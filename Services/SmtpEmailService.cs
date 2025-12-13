using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace ProjectM.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public SmtpEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendInvitationEmailAsync(string email, string token)
        {
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var port = int.Parse(_configuration["EmailSettings:Port"] ?? "587");
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var password = _configuration["EmailSettings:Password"];
                var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true");

                var smtpClient = new SmtpClient(smtpServer)
                {
                    Port = port,
                    Credentials = new NetworkCredential(senderEmail, password),
                    EnableSsl = enableSsl,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail!, "ProjectM Admin"),
                    Subject = "You have been invited to ProjectM",
                    Body = $"<h2>Welcome to ProjectM!</h2><p>You have been invited to join the platform.</p><p>Please use the following token to complete your registration:</p><h3>{token}</h3><p>Or click <a href='#'>here</a> (link implementation pending frontend).</p>",
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(email);

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // In a real app, log this error
                Console.WriteLine($"Failed to send email: {ex.Message}");
                // Do NOT throw, so the invitation process completes even if email fails
            }
        }
    }
}
