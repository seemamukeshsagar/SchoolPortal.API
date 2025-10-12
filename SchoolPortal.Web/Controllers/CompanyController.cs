using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SchoolPortal.API.DTOs.Company;
using SchoolPortal.API.DTOs;
using SchoolPortal.Web.Models.Company;

// Configure JSON serialization options
public static class JsonOptions
{
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };
}

namespace SchoolPortal.Web.Controllers
{
    [Authorize]
    public class CompanyController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CompanyController> _logger;
        private const int CacheDurationHours = 1;

        public CompanyController(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache,
            ILogger<CompanyController> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] + "/api/company";
            _cache = cache;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var token = HttpContext.Session.GetString("JWToken") ?? string.Empty;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var companies = await _cache.GetOrCreateAsync("AllCompanies", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(CacheDurationHours);
                    var response = await _httpClient.GetAsync(_apiBaseUrl);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<SchoolPortal.API.DTOs.Company.CompanyDto>>(content, JsonOptions.Default) 
                        ?? new List<SchoolPortal.API.DTOs.Company.CompanyDto>();
                });

                return View(companies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving companies");
                TempData["ErrorMessage"] = "Error retrieving companies. Please try again.";
                return View(new List<SchoolPortal.API.DTOs.Company.CompanyDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var token = HttpContext.Session.GetString("JWToken") ?? string.Empty;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var company = await _cache.GetOrCreateAsync($"Company_{id}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                    var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{id}");
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<SchoolPortal.API.DTOs.Company.CompanyDto>(content, JsonOptions.Default);
                });

                if (company == null)
                {
                    _logger.LogWarning("Company with ID {CompanyId} not found", id);
                    return NotFound();
                }

                return View(company);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving company with ID {CompanyId}", id);
                TempData["ErrorMessage"] = "Error retrieving company details.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStatesByCountry(Guid countryId)
        {
            try
            {
                var token = HttpContext.Session.GetString("JWToken") ?? string.Empty;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var states = await _cache.GetOrCreateAsync($"States_{countryId}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(CacheDurationHours);
                    var response = await _httpClient.GetAsync($"{_apiBaseUrl}/locations/states/{countryId}");
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<StateDto>>(content, JsonOptions.Default)
                        ?? new List<StateDto>();
                });

                return Json(states);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving states for country {CountryId}", countryId);
                return Json(new List<StateDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCitiesByState(Guid stateId)
        {
            try
            {
                var token = HttpContext.Session.GetString("JWToken") ?? string.Empty;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var cities = await _cache.GetOrCreateAsync($"Cities_{stateId}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(CacheDurationHours);
                    var response = await _httpClient.GetAsync($"{_apiBaseUrl}/locations/cities/{stateId}");
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<CityDto>>(content, JsonOptions.Default)
                        ?? new List<CityDto>();
                });

                return Json(cities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cities for state {StateId}", stateId);
                return Json(new List<CityDto>());
            }
        }

        private async Task LoadDropdownDataAsync(CompanyBaseViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            try
            {
                // Ensure Authorization header
                var token = HttpContext.Session.GetString("JWToken");
                if (!string.IsNullOrEmpty(token))
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Load Countries
                model.Countries = await GetCachedApiDataAsync<List<CountryDto>>(
                    "Countries",
                    $"{_apiBaseUrl}/locations/countries"
                ) ?? new List<CountryDto>();

                // Load States
                model.States = model.CountryId != Guid.Empty
                    ? await GetCachedApiDataAsync<List<StateDto>>(
                        $"States_{model.CountryId}",
                        $"{_apiBaseUrl}/locations/states/{model.CountryId}"
                      ) ?? new List<StateDto>()
                    : new List<StateDto>();

                // Load Cities + Jurisdiction Areas
                model.Cities = model.StateId != Guid.Empty
                    ? await GetCachedApiDataAsync<List<CityDto>>(
                        $"Cities_{model.StateId}",
                        $"{_apiBaseUrl}/locations/cities/{model.StateId}"
                      ) ?? new List<CityDto>()
                    : new List<CityDto>();

                model.JudistrictionAreas = model.Cities?.ToList() ?? new List<CityDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dropdown data");

                model.Countries = new List<CountryDto>();
                model.States = new List<StateDto>();
                model.Cities = new List<CityDto>();
                model.JudistrictionAreas = new List<CityDto>();
            }
        }

        /// <summary>
        /// Helper method that gets data from cache or API.
        /// </summary>
        private async Task<T?> GetCachedApiDataAsync<T>(string cacheKey, string apiUrl)
        {
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(CacheDurationHours);

                var response = await _httpClient.GetAsync(apiUrl).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonSerializer.Deserialize<T>(content, JsonOptions.Default);
            }).ConfigureAwait(false);
        }

    }
}