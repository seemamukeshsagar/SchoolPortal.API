using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SchoolPortal.API.DTOs;
using SchoolPortal.API.DTOs.Company;
using SchoolPortal.API.Interfaces;
using SchoolPortal.API.Interfaces.Services;
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
	[AllowAnonymous]
	public class CompanyController : Controller
	{
		private readonly HttpClient _httpClient;
		private readonly string _apiBaseUrl;        // for company CRUD
		private readonly string _locationsBaseUrl;  // for locations (states, cities, jurisdictions)
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

			var baseUrl = configuration["ApiSettings:BaseUrl"];
			if (baseUrl == null)
				throw new ArgumentNullException(nameof(baseUrl), "ApiSettings:BaseUrl configuration is missing.");

			baseUrl = baseUrl.TrimEnd('/');
			_apiBaseUrl = $"{baseUrl}/company";
			_locationsBaseUrl = $"{baseUrl}/locations";

			_cache = cache;
			_logger = logger;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			_logger.LogInformation("Entering Index action. API Base URL: {ApiBaseUrl}", _apiBaseUrl);
			
			try
			{
				var token = HttpContext.Session.GetString("JWToken") ?? string.Empty;
				_logger.LogDebug("Using token: {Token}", string.IsNullOrEmpty(token) ? "(empty)" : "[token present]");
				
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var companies = await _cache.GetOrCreateAsync("AllCompanies", async entry =>
				{
					_logger.LogInformation("Cache miss. Fetching companies from API...");
					entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(CacheDurationHours);
					
					try
					{
						_logger.LogDebug("Sending request to: {Url}", _apiBaseUrl);
						var response = await _httpClient.GetAsync(_apiBaseUrl);
						_logger.LogDebug("Received response with status code: {StatusCode}", response.StatusCode);
						
						var responseContent = await response.Content.ReadAsStringAsync();
						_logger.LogDebug("API Response: {Response}", responseContent);
						
						if (!response.IsSuccessStatusCode)
						{
							_logger.LogError("API returned {StatusCode} when fetching companies. Response: {Response}", 
								response.StatusCode, responseContent);
							return new List<SchoolPortal.API.DTOs.Company.CompanyDto>();
						}
						
						try
						{
							var result = JsonSerializer.Deserialize<List<SchoolPortal.API.DTOs.Company.CompanyDto>>(responseContent, JsonOptions.Default);
							_logger.LogInformation("Successfully deserialized {Count} companies", result?.Count ?? 0);
							return result ?? new List<SchoolPortal.API.DTOs.Company.CompanyDto>();
						}
						catch (JsonException jsonEx)
						{
							_logger.LogError(jsonEx, "Failed to deserialize companies response. Content: {Content}", responseContent);
							return new List<SchoolPortal.API.DTOs.Company.CompanyDto>();
						}
					}
					catch (HttpRequestException httpEx)
					{
						_logger.LogError(httpEx, "HTTP request failed when fetching companies. URL: {Url}", _apiBaseUrl);
						return new List<SchoolPortal.API.DTOs.Company.CompanyDto>();
					}
				});

				_logger.LogInformation("Returning {Count} companies to the view", companies?.Count ?? 0);
				
				if (companies == null || !companies.Any())
				{
					var message = "No companies found or there was an error loading companies.";
					_logger.LogWarning(message);
					TempData["InfoMessage"] = message;
				}

				return View(companies ?? new List<SchoolPortal.API.DTOs.Company.CompanyDto>());
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error in Index action");
				TempData["ErrorMessage"] = "An unexpected error occurred while loading companies. Please try again later.";
				return View(new List<SchoolPortal.API.DTOs.Company.CompanyDto>());
			}
		}

		[HttpGet]
		public async Task<IActionResult> Details(Guid id)
		{
			_logger.LogInformation("Entering Details action for company ID: {CompanyId}", id);
			
			try
			{
				var token = HttpContext.Session.GetString("JWToken") ?? string.Empty;
				_logger.LogDebug("Using token: {Token}", string.IsNullOrEmpty(token) ? "(empty)" : "[token present]");
				
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

		[HttpGet("[controller]/[action]")]
		[HttpGet("[controller]/[action]/{countryId}")]
		public async Task<IActionResult> GetStatesByCountry(Guid? countryId)
		{
			try
			{
				if (!countryId.HasValue || countryId == Guid.Empty)
				{
					return Json(new List<object>());
				}

				var token = HttpContext.Session.GetString("JWToken") ?? string.Empty;
				if (!string.IsNullOrWhiteSpace(token))
				{
					_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
				}

				var response = await _httpClient.GetAsync($"{_apiBaseUrl}/locations/states/{countryId.Value}");
				if (!response.IsSuccessStatusCode)
				{
					_logger.LogWarning("States API returned non-success status: {Status}", response.StatusCode);
					return Json(new List<object>());
				}

				var content = await response.Content.ReadAsStringAsync();
				var states = JsonSerializer.Deserialize<List<StateDto>>(content, JsonOptions.Default) ?? new List<StateDto>();
				var result = states.Select(s => (object)new { id = s.Id, name = s.StateName }).ToList();
				return Json(result);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving states for country {CountryId}", countryId);
				return Json(new List<object>());
			}
		}

		[HttpGet("[controller]/[action]")]
		[HttpGet("[controller]/[action]/{stateId}")]
		public async Task<IActionResult> GetCitiesByState(Guid? stateId)
		{
			try
			{
				if (!stateId.HasValue || stateId == Guid.Empty)
				{
					return Json(new List<CityDto>());
				}

				var token = HttpContext.Session.GetString("JWToken") ?? string.Empty;
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var cities = await _cache.GetOrCreateAsync($"Cities_{stateId.Value}", async entry =>
				{
					entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(CacheDurationHours);
					var response = await _httpClient.GetAsync($"{_apiBaseUrl}/locations/cities/{stateId.Value}");
					if (!response.IsSuccessStatusCode)
					{
						_logger.LogWarning("Cities API returned non-success status: {Status}", response.StatusCode);
						return new List<CityDto>();
					}
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

		[HttpGet("[controller]/[action]")]
		[HttpGet("[controller]/[action]/{stateId}")]
		public async Task<IActionResult> GetJurisdictionAreas(Guid? stateId)
		{
			try
			{
				if (!stateId.HasValue || stateId == Guid.Empty)
				{
					_logger.LogWarning("GetJurisdictionAreas called with null or empty stateId");
					return Json(new List<CityDto>());
				}

				_logger.LogInformation("Retrieving jurisdiction areas for state: {StateId}", stateId);

				var token = HttpContext.Session.GetString("JWToken") ?? string.Empty;
				if (!string.IsNullOrWhiteSpace(token))
				{
					_httpClient.DefaultRequestHeaders.Authorization =
						new AuthenticationHeaderValue("Bearer", token);
				}

				var response = await _httpClient.GetAsync($"{_locationsBaseUrl}/jurisdiction-areas/{stateId.Value}");
				if (!response.IsSuccessStatusCode)
				{
					_logger.LogWarning("Jurisdiction areas API returned status {Status} for state {StateId}", response.StatusCode, stateId);

					// Fallback to public company endpoint (cities) when unauthorized or any failure
					var citiesFallback = await _httpClient.GetAsync($"{_apiBaseUrl}/locations/cities/{stateId.Value}");
					if (!citiesFallback.IsSuccessStatusCode)
					{
						_logger.LogWarning("Fallback cities API also failed with status {Status}", citiesFallback.StatusCode);
						return Json(new List<CityDto>());
					}

					var fallbackContent = await citiesFallback.Content.ReadAsStringAsync();
					var fallbackAreas = JsonSerializer.Deserialize<List<CityDto>>(fallbackContent, JsonOptions.Default) ?? new List<CityDto>();
					return Json(fallbackAreas);
				}

				var content = await response.Content.ReadAsStringAsync();
				var areas = JsonSerializer.Deserialize<List<CityDto>>(content, JsonOptions.Default) ?? new List<CityDto>();
				return Json(areas);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error retrieving jurisdiction areas for state {StateId}", stateId);
				return Json(new List<CityDto>());
			}
		}

		private async Task LoadDropdownDataAsync(CompanyBaseViewModel model, bool loadCountries = true, bool loadStates = true, bool loadCities = true, bool loadJurisdictions = true)
		{
			if (model == null)
				throw new ArgumentNullException(nameof(model));

			// Initialize required properties if they're null
			model.CompanyName ??= string.Empty;
			model.Description ??= string.Empty;
			model.Address ??= string.Empty;
			model.ZipCode ??= string.Empty;
			model.Email ??= string.Empty;
			model.EstablishmentYear ??= string.Empty;
			
			// Only initialize collections if they're going to be loaded
			if (loadCountries) model.Countries ??= new List<CountryDto>();
			if (loadStates) model.States ??= new List<StateDto>();
			if (loadCities) model.Cities ??= new List<CityDto>();
			if (loadJurisdictions) model.JurisdictionAreas ??= new List<CityDto>();

			try
			{
				// Ensure Authorization header
				var token = HttpContext.Session.GetString("JWToken");
				if (!string.IsNullOrEmpty(token))
				{
					_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
				}

				// Load Countries if requested
				if (loadCountries)
				{
					model.Countries = await GetCachedApiDataAsync<List<CountryDto>>(
						"Countries",
						$"{_apiBaseUrl}/locations/countries"   // use company endpoint (no auth)
					) ?? new List<CountryDto>();
					
					_logger.LogInformation("Loaded {Count} countries", model.Countries.Count);
					
					// If no countries loaded, try direct API call
					if (model.Countries.Count == 0)
					{
						_logger.LogWarning("No countries loaded from cache, trying direct API call");
						try
						{
							var directResponse = await _httpClient.GetAsync($"{_apiBaseUrl}/locations/countries");
							if (directResponse.IsSuccessStatusCode)
							{
								var directContent = await directResponse.Content.ReadAsStringAsync();
								model.Countries = JsonSerializer.Deserialize<List<CountryDto>>(directContent, JsonOptions.Default) ?? new List<CountryDto>();
								_logger.LogInformation("Direct API call loaded {Count} countries", model.Countries.Count);
							}
						}
						catch (Exception ex)
						{
							_logger.LogError(ex, "Direct API call for countries also failed");
						}
					}
				}

				// Load States if CountryId is set and states are requested
				if (loadStates && model.CountryId != Guid.Empty)
				{
					_logger.LogInformation("Loading states for country: {CountryId}", model.CountryId);
					model.States = await GetCachedApiDataAsync<List<StateDto>>(
						$"States_Country_{model.CountryId}",
						$"{_apiBaseUrl}/locations/states/{model.CountryId}"
					) ?? new List<StateDto>();
					_logger.LogInformation("Loaded {Count} states", model.States.Count);
				}

				// Load Cities if StateId is set and cities are requested
				if (loadCities && model.StateId != Guid.Empty)
				{
					_logger.LogInformation("Loading cities for state: {StateId}", model.StateId);
					model.Cities = await GetCachedApiDataAsync<List<CityDto>>(
						$"Cities_State_{model.StateId}",
						$"{_apiBaseUrl}/locations/cities/{model.StateId}"
					) ?? new List<CityDto>();
					_logger.LogInformation("Loaded {Count} cities", model.Cities.Count);
				}

				// Load Jurisdiction Areas if requested and StateId is set
				if (loadJurisdictions && model.StateId != Guid.Empty)
				{
					try
					{
						_logger.LogInformation("Loading jurisdiction areas for state: {StateId}", model.StateId);
						model.JurisdictionAreas = await GetCachedApiDataAsync<List<CityDto>>(
							$"Jurisdictions_State_{model.StateId}",
							$"{_locationsBaseUrl}/jurisdiction-areas/{model.StateId}"
						) ?? new List<CityDto>();

						// If authorized call returns empty, fallback to public cities endpoint
						if (model.JurisdictionAreas.Count == 0)
						{
							_logger.LogWarning("Jurisdiction areas empty; falling back to cities for state {StateId}", model.StateId);
							model.JurisdictionAreas = await GetCachedApiDataAsync<List<CityDto>>(
								$"CitiesAsJurisdictions_State_{model.StateId}",
								$"{_apiBaseUrl}/locations/cities/{model.StateId}"
							) ?? new List<CityDto>();
						}

						_logger.LogInformation("Loaded {Count} jurisdiction areas", model.JurisdictionAreas.Count);
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Error loading jurisdiction areas for state {StateId}", model.StateId);
						_logger.LogInformation("Falling back to using cities as jurisdiction areas");
						model.JurisdictionAreas = model.Cities?.ToList() ?? new List<CityDto>();
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error loading dropdown data");
				// Don't override the model's existing values, just log the error
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

		[HttpGet]
		public async Task<IActionResult> Create()
		{
			try
			{
				var model = new CreateCompanyViewModel
				{
					CompanyName = string.Empty,
					Description = string.Empty,
					Address = string.Empty,
					ZipCode = string.Empty,
					Email = string.Empty,
					EstablishmentYear = string.Empty,
					IsActive = true,
					Countries = new List<CountryDto>(),
					States = new List<StateDto>(),
					Cities = new List<CityDto>(),
					JurisdictionAreas = new List<CityDto>()
				};
				
				await LoadDropdownDataAsync(model);
				return View(model);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error loading create company form");
				TempData["ErrorMessage"] = "Error loading company creation form. Please try again.";
				return RedirectToAction(nameof(Index));
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(CreateCompanyViewModel model)
		{
			if (!ModelState.IsValid)
			{
				await LoadDropdownDataAsync(model);
				return View(model);
			}

			try
			{
				var token = HttpContext.Session.GetString("JWToken") ?? string.Empty;
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var userIdStr = HttpContext.Session.GetString("User_Id");
				Guid? userId = Guid.TryParse(userIdStr, out var parsedUserId) ? parsedUserId : null;

				var createRequest = new SchoolPortal.API.DTOs.Company.CreateCompanyRequest
				{
					CompanyName = model.CompanyName ?? string.Empty,
					Description = model.Description ?? string.Empty,
					Address = model.Address ?? string.Empty,
					CountryId = model.CountryId,
					StateId = model.StateId,
					CityId = model.CityId,
					ZipCode = model.ZipCode ?? string.Empty,
					Email = model.Email ?? string.Empty,
					IsActive = model.IsActive,
					EstablishmentYear = model.EstablishmentYear ?? string.Empty,
					JurisdictionArea = Guid.Parse(model.JurisdictionArea),
					CreatedBy = userId,
					ModifiedBy = userId
				};

				var response = await _httpClient.PostAsJsonAsync(_apiBaseUrl, createRequest, JsonOptions.Default);
				response.EnsureSuccessStatusCode();

				// Invalidate the companies cache
				_cache.Remove("AllCompanies");

				TempData["SuccessMessage"] = "Company created successfully!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating company");
				ModelState.AddModelError("", "An error occurred while creating the company. Please try again.");
				await LoadDropdownDataAsync(model);
				return View(model);
			}
		}

		[HttpGet]
		public async Task<IActionResult> Edit(Guid id)
		{
			_logger.LogInformation("Entering Edit action for company ID: {CompanyId}", id);

			try
			{
				// Set authorization
				var token = HttpContext.Session.GetString("JWToken");
				if (!string.IsNullOrWhiteSpace(token))
					_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				// Try cache first
				var cacheKey = $"Company_{id}";
				var company = await _cache.GetOrCreateAsync(cacheKey, async entry =>
				{
					entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);

					var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{id}");
					if (!response.IsSuccessStatusCode)
					{
						_logger.LogWarning("Failed to fetch company {CompanyId}. StatusCode: {StatusCode}", id, response.StatusCode);
						return null;
					}

					var content = await response.Content.ReadAsStringAsync();
					return JsonSerializer.Deserialize<SchoolPortal.API.DTOs.Company.CompanyDto>(content, JsonOptions.Default);
				});

				// Build the view model (with or without company data)
				var viewModel = CreateCompanyViewModel(company, id);

				// Load dropdowns in sequence (country → state → city → jurisdiction)
				await LoadAllDropdownsAsync(viewModel);

				_logger.LogInformation(
					"Dropdowns loaded: Countries={CountryCount}, States={StateCount}, Cities={CityCount}, Jurisdictions={JurisdictionCount}",
					viewModel.Countries?.Count ?? 0,
					viewModel.States?.Count ?? 0,
					viewModel.Cities?.Count ?? 0,
					viewModel.JurisdictionAreas?.Count ?? 0
				);

				return View(viewModel);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error loading company for editing. ID: {CompanyId}", id);
				TempData["ErrorMessage"] = "Error loading company details for editing. Please try again.";
				return RedirectToAction(nameof(Index));
			}
		}

		/// <summary>
		/// Creates and initializes a company view model (either from DTO or blank template)
		/// </summary>
		private static UpdateCompanyViewModel CreateCompanyViewModel(SchoolPortal.API.DTOs.Company.CompanyDto? company, Guid id)
		{
			return new UpdateCompanyViewModel
			{
				Id = company?.Id ?? id,
				CompanyName = company?.CompanyName ?? string.Empty,
				Description = company?.Description ?? string.Empty,
				Address = company?.Address ?? string.Empty,
				Email = company?.Email ?? string.Empty,
				ZipCode = company?.ZipCode ?? string.Empty,
				EstablishmentYear = company?.EstablishmentYear ?? string.Empty,
				JurisdictionArea = company?.JurisdictionArea.ToString() ?? string.Empty,
				IsActive = company?.IsActive ?? false,
				CountryId = company?.CountryId ?? Guid.Empty,
				StateId = company?.StateId ?? Guid.Empty,
				CityId = company?.CityId ?? Guid.Empty,
				CountryName = company?.CountryName,
				StateName = company?.StateName,
				CityName = company?.CityName,
				JurisdictionAreaName = company?.JurisdictionAreaName,
				Status = company?.Status,
				StatusMessage = company?.StatusMessage
			};
		}

		/// <summary>
		/// Loads all dropdown data in proper dependency order.
		/// </summary>
		private async Task LoadAllDropdownsAsync(UpdateCompanyViewModel viewModel)
		{
			if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

			// 1. Load countries first
			await LoadDropdownDataAsync(viewModel, loadStates: false, loadCities: false, loadJurisdictions: false);
			
			// 2. Load states for the selected country
			if (viewModel.CountryId != Guid.Empty)
			{
				await LoadDropdownDataAsync(viewModel, loadCountries: false, loadCities: false, loadJurisdictions: false);
				
				// 3. Load cities for the selected state
				if (viewModel.StateId != Guid.Empty)
				{
					await LoadDropdownDataAsync(viewModel, loadCountries: false, loadStates: false, loadJurisdictions: false);
					
					// 4. Load jurisdictions for the selected state
					// Note: JurisdictionArea is a string, not a Guid
					if (!string.IsNullOrEmpty(viewModel.JurisdictionArea))
					{
						await LoadDropdownDataAsync(viewModel, 
							loadCountries: false, 
							loadStates: false, 
							loadCities: false, 
							loadJurisdictions: true);
					}
				}
			}

			// Jurisdiction loading is handled within LoadDropdownDataAsync
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(UpdateCompanyViewModel model)
		{
			_logger.LogInformation("Processing edit for company ID: {CompanyId}", model.Id);
			
			if (!ModelState.IsValid)
			{
				_logger.LogWarning("Model state is invalid for company ID: {CompanyId}", model.Id);
				await LoadDropdownDataAsync(model);
				return View(model);
			}

			try
			{
				var token = HttpContext.Session.GetString("JWToken") ?? string.Empty;
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var userIdStr = HttpContext.Session.GetString("User_Id");
				Guid? userId = Guid.TryParse(userIdStr, out var parsedUserId) ? parsedUserId : null;

				var updateRequest = new SchoolPortal.API.DTOs.Company.UpdateCompanyRequest
				{
					Id = model.Id,
					CompanyName = model.CompanyName ?? string.Empty,
					Description = model.Description ?? string.Empty,
					Address = model.Address ?? string.Empty,
					CountryId = model.CountryId,
					StateId = model.StateId,
					CityId = model.CityId,
					ZipCode = model.ZipCode ?? string.Empty,
					Email = model.Email ?? string.Empty,
					IsActive = model.IsActive,
					EstablishmentYear = model.EstablishmentYear ?? string.Empty,
					JurisdictionArea = Guid.Parse(model.JurisdictionArea),
					Status = model.Status ?? "INC",
					StatusMessage = model.StatusMessage ?? string.Empty,
					ModifiedBy = userId
				};

				var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/{model.Id}", updateRequest, JsonOptions.Default);
				response.EnsureSuccessStatusCode();

				// Invalidate caches
				_cache.Remove("AllCompanies");
				_cache.Remove($"Company_{model.Id}");

				TempData["SuccessMessage"] = "Company updated successfully!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating company with ID: {CompanyId}", model.Id);
				ModelState.AddModelError("", "An error occurred while updating the company. Please try again.");
				await LoadDropdownDataAsync(model);
				return View(model);
			}
		}
	}
}