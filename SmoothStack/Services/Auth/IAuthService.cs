using SmoothStack.Models.DTOs;

namespace SmoothStack.Services.Auth
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        Task<List<UserDto>> GetAllUsersAsync();
    }
}
