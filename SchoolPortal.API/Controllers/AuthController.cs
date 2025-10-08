using Microsoft.AspNetCore.Mvc;
using SchoolPortal.API.DTOs.Auth;
using SchoolPortal.API.Interfaces.Services;

namespace SchoolPortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
	private readonly IAuthService _authService;
	private readonly ILogger<AuthController> _logger;

	public AuthController(IAuthService authService, ILogger<AuthController> logger)
	{
		_authService = authService;
		_logger = logger;
	}

	[HttpPost("login")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> Login([FromBody] LoginRequest request)
	{
		try
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var response = await _authService.LoginAsync(request);
			return Ok(response);
		}
		catch (UnauthorizedAccessException ex)
		{
			_logger.LogWarning(ex, "Authentication failed for user: {Username}", request.Username);
			return Unauthorized(new { message = ex.Message });
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred during authentication for user: {Username}", request.Username);
			return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while processing your request." });
		}
	}

	[HttpPost("validate-token")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> ValidateToken([FromBody] string token)
	{
		try
		{
			var isValid = await _authService.ValidateTokenAsync(token);
			return isValid ? Ok() : Unauthorized();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error validating token");
			return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while validating the token." });
		}
	}

	[HttpGet("check-username/{username}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> CheckUsernameExists(string username)
	{
		if (string.IsNullOrWhiteSpace(username))
		{
			return BadRequest(new { message = "Username is required" });
		}

		try
		{
			var exists = await _authService.UserExistsAsync(username);
			return Ok(new { exists });
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error checking if username exists: {Username}", username);
			return StatusCode(StatusCodes.Status500InternalServerError,
				new { message = "An error occurred while checking username availability." });
		}
	}
}