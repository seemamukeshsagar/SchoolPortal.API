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
using SchoolPortal.Web.Services;  // For ICachingService
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace SchoolPortal.Web.Controllers
{
	public class AccountController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private const string ApiBaseUrl = "https://localhost:7185/api/";
		private readonly ILogger<AccountController> _logger;
		private readonly ICachingService _cachingService;
		private const string UserDetailsCachePrefix = "UserDetails_";
		private readonly IConfiguration _configuration;

		public AccountController(
			IHttpClientFactory httpClientFactory,
			ILogger<AccountController> logger,
			ICachingService cachingService,
			IConfiguration configuration)
		{
			_httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_cachingService = cachingService ?? throw new ArgumentNullException(nameof(cachingService));
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		}

		[HttpGet]
		[AllowAnonymous]
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
				
				// Optimized JSON serialization with source generation
				var loginRequest = new { model.UserName, model.Password };
				using var content = JsonContent.Create(loginRequest, options: new JsonSerializerOptions
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
					DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
				});

				// Set timeout for the request
				using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
				
				_logger.LogInformation("Attempting to authenticate user: {Username}", model.UserName);
				
				// Start both API calls in parallel
				var loginTask = client.PostAsync("Auth/login", content, cts.Token);
				var userDetailsTask = GetUserDetailsAsync(model.UserName, cts.Token);

				// Wait for both tasks to complete
				await Task.WhenAll(loginTask, userDetailsTask);
				var response = await loginTask;
				var userDetails = await userDetailsTask;

				if (response.IsSuccessStatusCode)
				{
					_logger.LogInformation("User {Username} authenticated successfully", model.UserName);

					// Invalidate any existing cache for this user
        			InvalidateUserCache(model.UserName);
					
					// Batch session updates
					if (userDetails != null)
					{
						var session = HttpContext.Session;
						session.SetString("User_FullName", userDetails.FirstName + ' ' + userDetails.LastName ?? string.Empty);
						if (userDetails.UserRoleId.HasValue)
						{
							session.SetString("User_RoleId", userDetails.UserRoleId.Value.ToString());
						}
						session.SetString("IsSuperUser", userDetails.IsSuperUser?.ToString() ?? "false");
						await session.CommitAsync(); // Single commit for all session changes
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

        // Update GetUserDetailsAsync to accept CancellationToken
        private async Task<UserDetail> GetUserDetailsAsync(string username, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be null or empty.", nameof(username));

            var cacheKey = $"{UserDetailsCachePrefix}{username}";
            var cacheDuration = TimeSpan.FromMinutes(_configuration.GetValue<int>("CacheSettings:UserDetailsCacheDurationMinutes"));

            return await _cachingService.GetOrCreateAsync(cacheKey, async () =>
            {
                using var client = _httpClientFactory.CreateClient("AuthApi");
                var response = await client.GetAsync(
                    $"User/GetUserDetails?username={Uri.EscapeDataString(username)}",
                    cancellationToken);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<UserDetail>(cancellationToken: cancellationToken);

                if (result == null)
                {
                    _logger.LogWarning("Received null user details for {Username}", username);
                    throw new InvalidOperationException($"No user details found for {username}.");
                }

                return result;
            }, cacheDuration);
        }

        // Add this method to invalidate cache when user data changes
        private void InvalidateUserCache(string username)
		{
			var cacheKey = $"{UserDetailsCachePrefix}{username}";
			_cachingService.Remove(cacheKey);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Logout()
		{
			// Clear session data
			HttpContext.Session.Clear();
			
			// Clear authentication cookies
			foreach (var cookie in Request.Cookies.Keys)
			{
				Response.Cookies.Delete(cookie);
			}
			
			// Sign out the user
			return SignOut(new AuthenticationProperties 
			{ 
				RedirectUri = Url.Action("Login", "Home") 
			}, CookieAuthenticationDefaults.AuthenticationScheme);
		}
		
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
