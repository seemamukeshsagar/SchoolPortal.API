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
					// Handle successful login (e.g., store token, set auth cookie)
					// For now, redirect to home
					if (Url.IsLocalUrl(returnUrl))
					{
						return Redirect(returnUrl);
					}
					return RedirectToAction("Index", "Home");
				}

				ModelState.AddModelError(string.Empty, "Invalid login attempt.");
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
			// Handle logout (e.g., clear auth cookies)
			return RedirectToAction("Index", "Home");
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
