using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SchoolPortal.API.DTOs.School;
using SchoolPortal.Web.Models.School;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Linq;

namespace SchoolPortal.Web.Controllers
{
    [Authorize]
    public class SchoolContactController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;
        private readonly IMemoryCache _cache;
        private readonly ILogger<SchoolContactController> _logger;
        private const string CacheKeyPrefix = "SchoolContacts_";

        public SchoolContactController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IMemoryCache cache,
            ILogger<SchoolContactController> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
            _cache = cache;
            _logger = logger;

            var baseUrl = _configuration["ApiSettings:BaseUrl"];
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new ArgumentException("API Base URL is not configured");
            }

            _apiBaseUrl = $"{baseUrl.TrimEnd('/')}/api/schoolcontact";
        }

        private async Task<string> GetTokenAsync()
        {
            return await Task.FromResult(HttpContext.Session.GetString("JWToken") ?? string.Empty);
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid schoolId)
        {
            try
            {
                var token = await GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Account");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var cacheKey = $"{CacheKeyPrefix}{schoolId}";
                if (!_cache.TryGetValue(cacheKey, out List<SchoolContactDto>? contacts) || contacts == null)
                {
                    var response = await _httpClient.GetAsync($"{_apiBaseUrl}/school/{schoolId}");
                    response.EnsureSuccessStatusCode();
                    
                    var content = await response.Content.ReadAsStringAsync();
                    var deserializedContacts = JsonConvert.DeserializeObject<List<SchoolContactDto>>(content);
                    contacts = deserializedContacts ?? new List<SchoolContactDto>();
                    
                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(30));
                    
                    _cache.Set(cacheKey, contacts, cacheOptions);
                }

                ViewBag.SchoolId = schoolId;
                return View(contacts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contacts for school {SchoolId}", schoolId);
                TempData["ErrorMessage"] = "An error occurred while retrieving contacts.";
                return RedirectToAction("Details", "School", new { id = schoolId });
            }
        }

        [HttpGet]
        public IActionResult Create(Guid schoolId)
        {
            var model = new SchoolContactViewModel
            {
                SchoolId = schoolId,
                IsActive = true
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SchoolContactViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var token = await GetTokenAsync();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var request = new SchoolContactRequest
                {
                    SchoolId = model.SchoolId,
                    FirstName = model.FirstName ?? throw new ArgumentNullException(nameof(model.FirstName), "First name is required"),
                    LastName = model.LastName ?? throw new ArgumentNullException(nameof(model.LastName), "Last name is required"),
                    Email = model.Email ?? string.Empty,
                    Phone = model.Phone ?? string.Empty,
                    MobilePhone = model.MobilePhone ?? string.Empty,
                    AddressLine1 = model.AddressLine1 ?? string.Empty,
                    AddressLine2 = model.AddressLine2 ?? string.Empty,
                    CityId = model.CityId,
                    StateId = model.StateId,
                    CountryId = model.CountryId,
                    IsActive = model.IsActive
                };

                var response = await _httpClient.PostAsJsonAsync(_apiBaseUrl, request);
                response.EnsureSuccessStatusCode();

                // Clear the cache for this school's contacts
                _cache.Remove($"{CacheKeyPrefix}{model.SchoolId}");

                TempData["SuccessMessage"] = "Contact created successfully.";
                return RedirectToAction("Index", new { schoolId = model.SchoolId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating contact for school {SchoolId}", model.SchoolId);
                ModelState.AddModelError("", "An error occurred while creating the contact. Please try again.");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var token = await GetTokenAsync();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{id}");
                response.EnsureSuccessStatusCode();

                var contact = await response.Content.ReadFromJsonAsync<SchoolContactDto>();
                if (contact == null)
                {
                    _logger.LogWarning("Contact not found for ID: {ContactId}", id);
                    return NotFound();
                }

                var model = new SchoolContactViewModel
                {
                    Id = contact.Id,
                    SchoolId = contact.SchoolId,
                    FirstName = contact.FirstName ?? string.Empty,
                    LastName = contact.LastName ?? string.Empty,
                    Email = contact.Email ?? string.Empty,
                    Phone = contact.Phone ?? string.Empty,
                    MobilePhone = contact.MobilePhone ?? string.Empty,
                    AddressLine1 = contact.AddressLine1 ?? string.Empty,
                    AddressLine2 = contact.AddressLine2 ?? string.Empty,
                    CityId = contact.CityId,
                    StateId = contact.StateId,
                    CountryId = contact.CountryId,
                    IsActive = contact.IsActive,
                    CityName = contact.CityName ?? string.Empty,
                    StateName = contact.StateName ?? string.Empty,
                    CountryName = contact.CountryName ?? string.Empty
                };

                if (TempData.ContainsKey("SchoolId"))
                {
                    TempData.Keep("SchoolId");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contact {ContactId}", id);
                TempData["ErrorMessage"] = "An error occurred while retrieving the contact.";
                return RedirectToAction("Index", new { schoolId = TempData["SchoolId"] });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SchoolContactViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid model");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var token = await GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Account");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var request = new SchoolContactRequest
                {
                    SchoolId = model.SchoolId,
                    FirstName = model.FirstName ?? string.Empty,
                    LastName = model.LastName ?? string.Empty,
                    Email = model.Email ?? string.Empty,
                    Phone = model.Phone ?? string.Empty,
                    MobilePhone = model.MobilePhone ?? string.Empty,
                    AddressLine1 = model.AddressLine1 ?? string.Empty,
                    AddressLine2 = model.AddressLine2 ?? string.Empty,
                    CityId = model.CityId,
                    StateId = model.StateId,
                    CountryId = model.CountryId,
                    IsActive = model.IsActive
                };

                if (TempData.ContainsKey("SchoolId"))
                {
                    TempData.Keep("SchoolId");
                }

                var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/{model.Id}", request);
                response.EnsureSuccessStatusCode();

                // Clear the cache for this school's contacts
                _cache.Remove($"{CacheKeyPrefix}{model.SchoolId}");

                TempData["SuccessMessage"] = "Contact updated successfully.";
                return RedirectToAction("Index", new { schoolId = model.SchoolId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating contact {ContactId}", model.Id);
                ModelState.AddModelError("", "An error occurred while updating the contact. Please try again.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, Guid schoolId)
        {
            try
            {
                var token = await GetTokenAsync();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/{id}");
                response.EnsureSuccessStatusCode();

                // Clear the cache for this school's contacts
                _cache.Remove($"{CacheKeyPrefix}{schoolId}");

                TempData["SuccessMessage"] = "Contact deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting contact {ContactId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the contact.";
            }

            return RedirectToAction("Index", new { schoolId });
        }
    }
}