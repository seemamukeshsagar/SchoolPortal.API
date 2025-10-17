using SchoolPortal.API.Models;

namespace SchoolPortal.API.Interfaces.Repositories;

public interface IUserRepository
{
    Task<UserDetail?> GetUserByUsernameAsync(string username);
    Task<UserDetail?> GetUserByIdAsync(Guid userId);
    Task<bool> ValidateUserCredentials(string username, string password);
    Task<IList<string>> GetUserRolesAsync(Guid userId);
    Task<IList<string>> GetUserPrivilegesAsync(Guid roleId);
    Task<bool> UserExistsAsync(string username);
    
    // New methods for optimized login
    Task<UserDetail?> GetUserForLoginAsync(string username);
    Task<(List<string> Roles, List<string> Privileges)> GetUserRolesAndPrivilegesAsync(Guid userId);
}
