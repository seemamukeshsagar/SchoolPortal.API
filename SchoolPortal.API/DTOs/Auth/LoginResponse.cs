namespace SchoolPortal.API.DTOs.Auth;

public class LoginResponse
{
	public Guid UserId { get; set; }
	public string Username { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public string Token { get; set; } = string.Empty;
	public DateTime Expiration { get; set; }
	public List<string> Roles { get; set; } = new();
	public List<string> Privileges { get; set; } = new();
}
