using Microsoft.EntityFrameworkCore;
using SchoolPortal.API.Interfaces.Repositories;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Repositories;

public class UserRepository : IUserRepository
{
	private readonly SchoolPortalNewContext _context;

	public UserRepository(SchoolPortalNewContext context)
	{
		_context = context;
	}

	public async Task<UserDetail?> GetUserByUsernameAsync(string username)
	{
		return await _context.UserDetails
			.Include(u => u.UserRole)
			.ThenInclude(r => r.RolePrivileges.Where(rp => rp.IsActive && !rp.IsDeleted && rp.Privilege != null))
			.ThenInclude(rp => rp.Privilege)
			.FirstOrDefaultAsync(u => u.UserName == username && !u.IsDeleted && u.IsActive);
	}

	public async Task<bool> ValidateUserCredentials(string username, string password)
	{
		// In a real application, use proper password hashing (e.g., BCrypt)
		var user = await GetUserByUsernameAsync(username);
		if (user == null) return false;
		
		// This is a simple comparison for demonstration
		// In production, use password hashing like BCrypt.Verify(password, user.UserPassword)
		return user.UserPassword == password;
	}

	public async Task<IList<string>> GetUserRolesAsync(Guid userId)
	{
		// This is a simplified example. Adjust according to your role management system
		var roles = new List<string>();
		
		var user = await _context.UserDetails
			.Include(u => u.UserRole) // Assuming there's a navigation property
			.FirstOrDefaultAsync(u => u.Id == userId);

		if (user?.UserRole != null)
		{
			// Add role name to the list. Adjust property names as per your model
			roles.Add(user.UserRole.Name ?? "User");
		}
		
		return roles;
	}

    public async Task<IList<string>> GetUserPrivilegesAsync(Guid roleId)
	{
		var privileges = await _context.RolePrivileges
            .Where(rp => rp.RoleId == roleId && rp.IsActive && !rp.IsDeleted)
            .Include(rp => rp.Privilege)
            .Where(rp => rp.Privilege != null && 
                        rp.Privilege.IsActive && 
                        !rp.Privilege.IsDeleted &&
                        !string.IsNullOrEmpty(rp.Privilege.PrivilegeName))
            .Select(rp => rp.Privilege!.PrivilegeName)
            .ToListAsync();

        return privileges!;
	}

	public async Task<bool> UserExistsAsync(string username)
	{
		return await _context.UserDetails
			.AnyAsync(u => u.UserName == username && !u.IsDeleted && u.IsActive);
	}

	public async Task<UserDetail?> GetUserForLoginAsync(string username)
	{
		return await _context.UserDetails
			.Where(u => u.UserName == username && !u.IsDeleted && u.IsActive)
			.Select(u => new UserDetail
			{
				Id = u.Id,
				UserName = u.UserName,
				EmailAddress = u.EmailAddress,
				FirstName = u.FirstName,
				LastName = u.LastName,
				//PasswordHash = u.UserPassword, // This should be a hashed password
				IsActive = u.IsActive,
				IsDeleted = u.IsDeleted,
				UserRoleId = u.UserRoleId
			})
			.FirstOrDefaultAsync(u => u.UserName == username && !u.IsDeleted && u.IsActive);
	}

	public async Task<UserDetail?> GetUserByIdAsync(Guid userId)
	{
		return await _context.UserDetails
			.Where(u => u.Id == userId && !u.IsDeleted && u.IsActive)
			.Select(u => new UserDetail
			{
				Id = u.Id,
				UserName = u.UserName,
				EmailAddress = u.EmailAddress,
				FirstName = u.FirstName,
				LastName = u.LastName,
				IsActive = u.IsActive,
				IsDeleted = u.IsDeleted,
				UserRoleId = u.UserRoleId
			})
			.FirstOrDefaultAsync();
	}

	public async Task<(List<string> Roles, List<string> Privileges)> GetUserRolesAndPrivilegesAsync(Guid userId)
	{
		var result = await _context.UserDetails
			.Where(u => u.Id == userId)
			.Select(u => new
			{
				Roles = u.UserRole != null ? new[] { u.UserRole.Name ?? "User" } : Array.Empty<string>(),
				Privileges = u.UserRole.RolePrivileges
					.Where(rp => rp.IsActive && !rp.IsDeleted && 
								rp.Privilege != null && 
								rp.Privilege.IsActive && 
								!rp.Privilege.IsDeleted)
					.Select(rp => rp.Privilege.PrivilegeName)
					.Where(name => !string.IsNullOrEmpty(name))
					.ToList()
			})
			.FirstOrDefaultAsync();

		return (result?.Roles?.ToList() ?? new List<string>(), 
				result?.Privileges ?? new List<string>());
	}
}
