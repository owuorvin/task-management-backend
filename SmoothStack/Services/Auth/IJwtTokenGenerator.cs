using SmoothStack.Models;

namespace SmoothStack.Services.Auth
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
