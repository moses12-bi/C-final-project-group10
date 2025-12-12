using ProjectM.Models;

namespace ProjectM.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user, List<string> permissions);
        Guid? ValidateToken(string token);
        string? GetUserIdFromToken(string token);
    }
}
