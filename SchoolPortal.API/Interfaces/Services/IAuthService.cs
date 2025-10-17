using SchoolPortal.API.DTOs.Auth;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<bool> ValidateTokenAsync(string token);
    Task<bool> UserExistsAsync(string username);
    Task<UserDetail?> GetUserByIdAsync(Guid userId);
    Task<(List<string> Roles, List<string> Privileges)> GetUserRolesAndPrivilegesAsync(Guid userId);
}
