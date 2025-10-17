using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SchoolPortal.Web.Models;
using SchoolPortal.Web.Models.Privilege;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SchoolPortal.Web.Controllers
{
    [Authorize]
    public class PrivilegeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public PrivilegeController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var apiBaseUrl = _configuration["APIBaseUrl"];
            var response = await _httpClient.GetAsync($"{apiBaseUrl}/api/Privilege");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var privileges = JsonSerializer.Deserialize<List<PrivilegeListViewModel>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(privileges);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePrivilegeViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var content = new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json");

            var apiBaseUrl = _configuration["APIBaseUrl"];
            var response = await _httpClient.PostAsync($"{apiBaseUrl}/api/Privilege", content);
            
            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            var errorContent = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, "An error occurred while creating the privilege.");
            
            return View(model);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var apiBaseUrl = _configuration["APIBaseUrl"];
            var response = await _httpClient.GetAsync($"{apiBaseUrl}/api/Privilege/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var content = await response.Content.ReadAsStringAsync();
            var privilege = JsonSerializer.Deserialize<EditPrivilegeViewModel>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (privilege == null)
                return NotFound();

            return View(privilege);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditPrivilegeViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var content = new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json");

            var apiBaseUrl = _configuration["APIBaseUrl"];
            var response = await _httpClient.PutAsync($"{apiBaseUrl}/api/Privilege/{model.Id}", content);
            
            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            var errorContent = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, "An error occurred while updating the privilege.");
            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var apiBaseUrl = _configuration["APIBaseUrl"];
            var response = await _httpClient.DeleteAsync($"{apiBaseUrl}/api/Privilege/{id}");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return BadRequest("An error occurred while deleting the privilege.");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}