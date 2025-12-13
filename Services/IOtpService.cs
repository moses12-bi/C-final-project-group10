namespace ProjectM.Services
{
    public interface IOtpService
    {
        string GenerateOtp(string key);
        bool ValidateOtp(string key, string otp);
    }
}
