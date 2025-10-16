using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SchoolPortal.API.Interfaces.Services;
using SchoolPortal.API.Models;
using SchoolPortal.Web.DTOs;
using SchoolPortal.Web.Models.Account;
using SchoolPortal.Web.Services;

namespace SchoolPortal.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private const string UserDetailsCachePrefix = "UserDetails_";
        
        private readonly ILogger<AccountController> _logger;
        private readonly IAuthService _authService;
        private readonly IUserProfileService _userProfileService;
        private readonly ILoginAttemptService _loginAttemptService;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;  // Changed from ICachingService to IMemoryCache

        public AccountController(
            ILogger<AccountController> logger,
            IAuthService authService,
            IUserProfileService userProfileService,
            ILoginAttemptService loginAttemptService,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache)  // Added IMemoryCache
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _userProfileService = userProfileService ?? throw new ArgumentNullException(nameof(userProfileService));
            _loginAttemptService = loginAttemptService ?? throw new ArgumentNullException(nameof(loginAttemptService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        [HttpGet("login")]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToLocal(returnUrl);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost("login")]
		[AllowAnonymous]
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
				// Check if the user is already authenticated
				if (User.Identity?.IsAuthenticated == true)
				{
					return RedirectToLocal(returnUrl);
				}

				// Check login attempts
				if (await _loginAttemptService.IsAccountLocked(model.UserName))
				{
					ModelState.AddModelError(string.Empty, "Too many failed login attempts. Please try again later.");
					return View(model);
				}

				// Authenticate the user
				var loginRequest = new SchoolPortal.API.DTOs.Auth.LoginRequest
				{
					Username = model.UserName,
					Password = model.Password
				};

				try
				{
					var authResult = await _authService.LoginAsync(loginRequest);

					if (authResult == null || string.IsNullOrEmpty(authResult.Token))
					{
						await _loginAttemptService.RecordFailedAttempt(model.UserName);
						ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check your credentials.");
						return View(model);
					}

					// Reset failed attempts on successful login
					await _loginAttemptService.ResetAttempts(model.UserName);

					// Create claims
					var claims = new List<Claim>
					{
						new Claim(ClaimTypes.Name, authResult.Username),
						new Claim(ClaimTypes.NameIdentifier, authResult.UserId.ToString()),
						new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
					};

					// Add roles to claims if available
					if (authResult.Roles?.Any() == true)
					{
						claims.AddRange(authResult.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
					}

					var claimsIdentity = new ClaimsIdentity(
						claims, CookieAuthenticationDefaults.AuthenticationScheme);

					var authProperties = new AuthenticationProperties
					{
						AllowRefresh = true,
						IsPersistent = model.RememberMe,
						ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
					};

					await HttpContext.SignInAsync(
						CookieAuthenticationDefaults.AuthenticationScheme,
						new ClaimsPrincipal(claimsIdentity),
						authProperties);

					// Log the successful login
					_logger.LogInformation("User {Username} logged in at {Time}", 
						model.UserName, DateTime.UtcNow);

					// Store user details in session if needed
					if (!string.IsNullOrEmpty(authResult.FirstName) || !string.IsNullOrEmpty(authResult.LastName))
					{
						var fullName = $"{authResult.FirstName} {authResult.LastName}".Trim();
						var role = authResult.Roles?.FirstOrDefault() ?? "User";
						
						HttpContext.Session.SetString("User_FullName", fullName);
						HttpContext.Session.SetString("User_Role", role);
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error during authentication for user {Username}", model.UserName);
					ModelState.AddModelError(string.Empty, "An error occurred during authentication. Please try again.");
					return View(model);
				}

				// Handle return URL
				if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
				{
					return Redirect(returnUrl);
				}

				return RedirectToAction("Index", "Home");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error during login for user {Username}", model.UserName);
				ModelState.AddModelError(string.Empty, "An error occurred while processing your request. Please try again.");
				return View(model);
			}
		}

        [HttpPost("logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var username = User.Identity?.Name;
            
            // Clear session
            HttpContext.Session.Clear();
            
            // Clear authentication cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            // Clear all cookies
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            _logger.LogInformation("User {Username} logged out at {Time}", 
                username, DateTime.UtcNow);

            return RedirectToAction("Login");
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            try
            {
                var username = User.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { success = false, message = "User not authenticated" });
                }

                var userProfile = await _userProfileService.GetUserProfileAsync(username);
                return Json(userProfile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user profile for {Username}", User.Identity?.Name);
                return StatusCode(
                    (int)HttpStatusCode.InternalServerError, 
                    new { success = false, message = "An error occurred while fetching user profile" });
            }
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        // Add this method to invalidate cache when user data changes
        private void InvalidateUserCache(string username)
		{
			if (string.IsNullOrWhiteSpace(username))
                return;
                
			var cacheKey = $"UserDetails_{username}";
			_cache.Remove(cacheKey);
		}

		[HttpGet]
		public IActionResult AccessDenied()
		{
			return View();
		}
	}

	public class UserSession
	{
		public string? FullName { get; set; }
		public Guid? UserRoleId { get; set; }
		public bool? IsSuperUser { get; set; }
	}
}
