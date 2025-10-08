using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SchoolPortal.API.DTOs.Auth;
using SchoolPortal.API.Interfaces.Repositories;
using SchoolPortal.API.Interfaces.Services;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var isValidUser = await _userRepository.ValidateUserCredentials(request.Username, request.Password);
        if (!isValidUser)
        {
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        var user = await _userRepository.GetUserByUsernameAsync(request.Username);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

        var roles = await _userRepository.GetUserRolesAsync(user.Id);
        if (user.UserRoleId == null)
        {
            throw new InvalidOperationException("User does not have an assigned role");
        }

        var privileges = await _userRepository.GetUserPrivilegesAsync((Guid)user.UserRoleId);
        var token = GenerateJwtToken(user, roles);

        return new LoginResponse
        {
            UserId = user.Id,
            Username = user.UserName,
            Email = user.EmailAddress,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = token.ValidTo,
            Roles = roles.ToList(),
            Privileges = privileges.ToList()
        };
    }

    private JwtSecurityToken GenerateJwtToken(UserDetail user, IList<string> roles)
    {
        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        // Add roles to the claims
        foreach (var role in roles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["JWT:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured")));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }

    public Task<bool> ValidateTokenAsync(string token)
    {
        // Implement token validation logic here
        // This is a simplified example
        return Task.FromResult(!string.IsNullOrEmpty(token));
    }

    public async Task<bool> UserExistsAsync(string username)
    {
        return await _userRepository.UserExistsAsync(username);
    }
}
