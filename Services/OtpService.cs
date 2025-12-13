using Microsoft.Extensions.Caching.Memory;

namespace ProjectM.Services
{
    public class OtpService : IOtpService
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _expiry = TimeSpan.FromMinutes(5);

        public OtpService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public string GenerateOtp(string key)
        {
            // Generate a 6-digit OTP
            var otp = new Random().Next(100000, 999999).ToString();

            // Store in cache
            _cache.Set(key, otp, _expiry);

            return otp;
        }

        public bool ValidateOtp(string key, string otp)
        {
            if (_cache.TryGetValue(key, out string? storedOtp))
            {
                if (storedOtp == otp)
                {
                    _cache.Remove(key); // Invalidate after successful use
                    return true;
                }
            }
            return false;
        }
    }
}
