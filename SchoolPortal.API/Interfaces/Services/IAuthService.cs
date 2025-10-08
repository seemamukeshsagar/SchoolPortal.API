using SchoolPortal.API.DTOs.Auth;

namespace SchoolPortal.API.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<bool> ValidateTokenAsync(string token);
    Task<bool> UserExistsAsync(string username);
}
