namespace ProjectM.Services
{
    public class MockEmailService : IEmailService
    {
        public async Task SendInvitationEmailAsync(string email, string token)
        {
            // Mock implementation - in production, this would send a real email
            Console.WriteLine($"MOCK EMAIL: Invitation sent to {email}");
            Console.WriteLine($"Token: {token}");
            Console.WriteLine($"Registration link: https://yourapp.com/register?token={token}");
            
            // Simulate email sending delay
            await Task.Delay(100);
            
            // In a real implementation, you would use services like:
            // - SendGrid
            // - SMTP
            // - Azure Communication Services
            // - AWS SES
        }
    }
}
