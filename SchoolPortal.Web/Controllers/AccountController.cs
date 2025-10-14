using Microsoft.AspNetCore.Mvc;
using SchoolPortal.Web.Models.Account;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using SchoolPortal.API.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace SchoolPortal.Web.Controllers
{
	public class AccountController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private const string ApiBaseUrl = "https://localhost:7185/api/";
		private readonly ILogger<AccountController> _logger;

		public AccountController(IHttpClientFactory httpClientFactory, ILogger<AccountController> logger)
		{
			_httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[HttpGet]
		public IActionResult Login(string? returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			try
			{
				using var client = _httpClientFactory.CreateClient("AuthApi");
				
				// Create request content with optimized JSON serialization
				var jsonOptions = new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true,
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase
				};
				using var content = JsonContent.Create(new { model.UserName, model.Password }, options: jsonOptions);
				
				// Set timeout for the request
				using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
				
				_logger.LogInformation("Attempting to authenticate user: {Username}", model.UserName);
				
				using var response = await client.PostAsync("Auth/login", content, cts.Token);
				
				if (response.IsSuccessStatusCode)
				{
					_logger.LogInformation("User {Username} authenticated successfully", model.UserName);
					
					// Get user details and store in session
					var userDetails = await GetUserDetailsAsync(model.UserName);
					if (userDetails != null)
					{
						HttpContext.Session.SetString("User_FullName", userDetails.FullName ?? string.Empty);
						if (userDetails.UserRoleId.HasValue)
						{
							HttpContext.Session.SetString("User_RoleId", userDetails.UserRoleId.Value.ToString());
						}
						HttpContext.Session.SetString("IsSuperUser", userDetails.IsSuperUser?.ToString() ?? "false");
					}
					
					return Url.IsLocalUrl(returnUrl) 
						? Redirect(returnUrl) 
						: RedirectToAction("Index", "Home");
				}

				_logger.LogWarning("Failed login attempt for user: {Username}", model.UserName);
				ModelState.AddModelError(string.Empty, "Invalid username or password.");
				return View(model);
			}
			catch (TaskCanceledException ex) when (!ex.CancellationToken.IsCancellationRequested)
			{
				_logger.LogError(ex, "Login request timed out for user: {Username}", model.UserName);
				ModelState.AddModelError(string.Empty, "The request timed out. Please try again.");
			}
			catch (HttpRequestException ex)
			{
				_logger.LogError(ex, "Error connecting to authentication service for user: {Username}", model.UserName);
				ModelState.AddModelError(string.Empty, "Unable to connect to the authentication service. Please try again later.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error during login for user: {Username}", model.UserName);
				ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
			}

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Logout()
		{
			// Clear session data
			HttpContext.Session.Clear();
			// Clear authentication cookies
			// ... (your existing logout logic)
			return RedirectToAction("Index", "Home");
		}
		
		/*private async Task<UserSession> GetUserDetailsAsync(string username)
		{
			try
			{
				using var client = _httpClientFactory.CreateClient("AuthApi");
				var response = await client.GetAsync($"Users/{username}");
				
				if (response.IsSuccessStatusCode)
				{
					_logger.LogInformation("User {Username} authenticated successfully", model.UserName);

					// Parse login response to capture UserId and token for session
					var loginJson = await response.Content.ReadAsStringAsync();
					var loginResp = JsonSerializer.Deserialize<SchoolPortal.API.DTOs.Auth.LoginResponse>(
						loginJson,
						new JsonSerializerOptions { PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
					);
					if (loginResp != null)
					{
						HttpContext.Session.SetString("User_Id", loginResp.UserId.ToString());
						if (!string.IsNullOrWhiteSpace(loginResp.Token))
						{
							HttpContext.Session.SetString("JWToken", loginResp.Token);
						}
					}

					// Get user details and store in session
					var userDetails = await GetUserDetailsAsync(model.UserName);
					if (userDetails != null)
					{
						HttpContext.Session.SetString("User_FullName", userDetails.FullName);
						if (userDetails.UserRoleId.HasValue)
						{
							HttpContext.Session.SetString("User_RoleId", userDetails.UserRoleId.Value.ToString());
						}
						HttpContext.Session.SetString("IsSuperUser", userDetails.IsSuperUser?.ToString() ?? "false");
					}

					return Url.IsLocalUrl(returnUrl)
						? Redirect(returnUrl)
						: RedirectToAction("Index", "Home");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching user details for {Username}", username);
				return new UserSession { FullName = username };
			}
		}*/

		private async Task<UserSession> GetUserDetailsAsync(string username)
		{
			try
			{
				using var client = _httpClientFactory.CreateClient("AuthApi");
				var response = await client.GetAsync($"Users/{username}");
				
				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					var options = new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true,
						PropertyNamingPolicy = JsonNamingPolicy.CamelCase
					};
					
					var userDetail = JsonSerializer.Deserialize<UserDetail>(content, options);
					return new UserSession 
					{ 
						FullName = $"{userDetail?.FirstName} {userDetail?.LastName}".Trim(),
						UserRoleId = userDetail?.UserRoleId,
						IsSuperUser = userDetail?.IsSuperUser
					};
				}

				_logger.LogWarning("Failed to fetch user details for {Username}", username);
				return new UserSession { FullName = username };
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching user details for {Username}", username);
				return new UserSession { FullName = username };
			}
		}

		private IActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			return RedirectToAction(nameof(HomeController.Index), "Home");
		}

		[HttpGet]
		public IActionResult AccessDenied()
		{
			return View();
		}

	}
}
