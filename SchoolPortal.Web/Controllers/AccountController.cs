using Microsoft.AspNetCore.Mvc;
using SchoolPortal.Web.Models.Account;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Controllers
{
	public class AccountController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private const string ApiBaseUrl = "https://localhost:7185/api/"; // Update with your API URL

		public AccountController(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
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
                var client = _httpClientFactory.CreateClient();
                var content = new StringContent(
                    JsonSerializer.Serialize(new { model.UserName, model.Password }),
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PostAsync($"{ApiBaseUrl}Auth/login", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    string firstName = loginResponse.FirstName ?? string.Empty;
                    string lastName = loginResponse.LastName ?? string.Empty;
                    string fullName = (firstName + " " + lastName).Trim();
                    if (String.IsNullOrEmpty(fullName.Trim()))
                    {
                        fullName = "Guest";
                    }
                    // Store user details in session
                    var userSession = new UserSession
                    {
                        UserId = loginResponse.UserId,
                        UserName = loginResponse.UserName,
                        Email = loginResponse.Email,
                        FullName = fullName,
                        Roles = loginResponse.Roles ?? new List<string>(),
                        Privileges = loginResponse.Privileges ?? new List<string>()
                    };

                    // Store session
                    HttpContext.Session.SetString("UserSession", JsonSerializer.Serialize(userSession));

                    // Store user roles in session for easy access
                    HttpContext.Session.SetString("UserRoles", string.Join(",", userSession.Roles));

                    // Redirect to return URL or home
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }
            catch (Exception ex)
            {
                // Log the error
                Debug.WriteLine($"Login error: {ex.Message}");
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            // Clear the session
            HttpContext.Session.Clear();
            
            // Clear any existing external cookie
            // If you're using cookie authentication, add this:
            // await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            return RedirectToAction("Index", "Home");
        }

        // Helper method to get current user from session
        private UserSession GetCurrentUser()
        {
            var userSession = HttpContext.Session.GetString("UserSession");
            if (string.IsNullOrEmpty(userSession))
                return null;

            return JsonSerializer.Deserialize<UserSession>(userSession);
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
