<<<<<<< HEAD
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
=======
﻿using Microsoft.AspNetCore.Authorization;
>>>>>>> 0e5f10e9c378fdd6a6a0df3d0162372e3d96a58f
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SchoolPortal.API.DTOs;
using SchoolPortal.API.DTOs.School;
using SchoolPortal.Web.Models.School;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CompanyDto = SchoolPortal.API.DTOs.Company.CompanyDto;
using System.Text.Json;

namespace SchoolPortal.Web.Controllers
{
	// [Authorize]
	public class SchoolController : Controller
	{
		private readonly HttpClient _httpClient;
		private readonly IConfiguration _configuration;
		private readonly string _apiBaseUrl;
		private readonly string _companyApiBaseUrl;
        private readonly string _locationsBaseUrl;  // for locations (states, cities, jurisdictions)
        private readonly IMemoryCache _cache;
        private readonly ILogger<SchoolController> _logger;
        private const int CacheDurationHours = 1;


        public SchoolController(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache,
            ILogger<SchoolController> logger)
        {
            _httpClient = httpClientFactory.CreateClient();

            var baseUrl = configuration["ApiSettings:BaseUrl"];
            if (baseUrl == null)
                throw new ArgumentNullException(nameof(baseUrl), "ApiSettings:BaseUrl configuration is missing.");

            baseUrl = baseUrl.TrimEnd('/');
            _apiBaseUrl = $"{baseUrl}/school";
			_companyApiBaseUrl = $"{baseUrl}/company";
			_locationsBaseUrl = $"{baseUrl}/locations";

            _cache = cache;
            _logger = logger;
        }

		private async Task<T?> GetCachedApiDataAsync<T>(string cacheKey, string apiUrl)
		{
<<<<<<< HEAD
			_configuration = configuration;
			_httpClient = httpClientFactory.CreateClient();
			_apiBaseUrl = _configuration["ApiSettings:BaseUrl"] + "school";
			_companyApiBaseUrl = _configuration["ApiSettings:BaseUrl"] + "company";
=======
			return await _cache.GetOrCreateAsync(cacheKey, async entry =>
			{
				entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(CacheDurationHours);

				var response = await _httpClient.GetAsync(apiUrl).ConfigureAwait(false);
				response.EnsureSuccessStatusCode();

				var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				return JsonSerializer.Deserialize<T>(content, JsonOptions.Default);
			}).ConfigureAwait(false);
>>>>>>> 0e5f10e9c378fdd6a6a0df3d0162372e3d96a58f
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

				var schools = await _cache.GetOrCreateAsync("AllSchools", async entry =>
				{
					_logger.LogInformation("Cache miss. Fetching schools from API...");
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
							_logger.LogError("API returned {StatusCode} when fetching schools. Response: {Response}",
								response.StatusCode, responseContent);
							return new List<SchoolDto>();
						}

						try
						{
							var result = System.Text.Json.JsonSerializer.Deserialize<List<SchoolDto>>(responseContent, JsonOptions.Default);
							_logger.LogInformation("Successfully deserialized {Count} schools", result != null ? result.Count : 0);
							return result ?? new List<SchoolDto>();
						}
						catch (System.Text.Json.JsonException jsonEx)
						{
							_logger.LogError(jsonEx, "Failed to deserialize schools response. Content: {Content}", responseContent);
							return new List<SchoolDto>();
						}
					}
					catch (HttpRequestException httpEx)
					{
						_logger.LogError(httpEx, "HTTP request failed when fetching schools. URL: {Url}", _apiBaseUrl);
						return new List<SchoolDto>();
					}
				});

				var count = schools != null ? schools.Count : 0;
				_logger.LogInformation("Returning {Count} schools to the view", count);

				if (schools == null || count == 0)
				{
					var message = "No schools found or there was an error loading schools.";
					_logger.LogWarning(message);
					TempData["InfoMessage"] = message;
				}

				return View(schools ?? new List<SchoolDto>());
			}
			catch (Exception ex)
			{
<<<<<<< HEAD
				string temp = ex.Message + "\n" + ex.StackTrace;
				TempData["ErrorMessage"] = "Error retrieving schools. Please try again.";
=======
				_logger.LogError(ex, "Unexpected error in Index action");
				TempData["ErrorMessage"] = "An unexpected error occurred while loading schools. Please try again later.";
>>>>>>> 0e5f10e9c378fdd6a6a0df3d0162372e3d96a58f
				return View(new List<SchoolDto>());
			}
		}

		[HttpGet]
		public async Task<IActionResult> Details(Guid id)
		{
			_logger.LogInformation("Entering Details action for school ID: {SchoolId}", id);
			
			try
			{
				var token = HttpContext.Session.GetString("JWToken") ?? string.Empty;
				_logger.LogDebug("Using token: {Token}", string.IsNullOrEmpty(token) ? "(empty)" : "[token present]");
				
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var school = await _cache.GetOrCreateAsync($"School_{id}", async entry =>
				{
					entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
					var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{id}");
					response.EnsureSuccessStatusCode();
					var content = await response.Content.ReadAsStringAsync();
					return System.Text.Json.JsonSerializer.Deserialize<SchoolDto>(content, JsonOptions.Default);
				});

				if (school == null)
				{
					_logger.LogWarning("School with ID {SchoolId} not found", id);
					return NotFound();
				}

				return View(school);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving school with ID {SchoolId}", id);
				TempData["ErrorMessage"] = "Error retrieving school details.";
				return RedirectToAction(nameof(Index));
			}
		}

		[HttpGet]
		public async Task<IActionResult> Create()
		{
			try
			{
				var token = HttpContext.Session.GetString("JWToken");
				if (!string.IsNullOrEmpty(token))
				{
					_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
				}

				var model = new CreateSchoolViewModel
				{
					Name = string.Empty,
					Description = string.Empty,
					Address1 = string.Empty,
					Address2 = string.Empty,
					Email = string.Empty,
					Phone = string.Empty,
					Mobile = string.Empty,
					EstablishmentYear = string.Empty,
					BankName = string.Empty,
					BankAddress1 = string.Empty,
					BankAddress2 = string.Empty,
					BankZipCode = string.Empty,
					AccountNumber = string.Empty,
					ZipCode = string.Empty,
					IsActive = true,
                    // Initialize collections to prevent null reference exceptions
                    Companies = new List<CompanyDto>(),
                    Countries = new List<CountryDto>(),
                    States = new List<StateDto>(),
                    Cities = new List<CityDto>(),
                    JudistrictionCountries = new List<CountryDto>(),
                    JudistrictionStates = new List<StateDto>(),
                    JudistrictionCities = new List<CityDto>(),
                    BankCountries = new List<CountryDto>(),
                    BankStates = new List<StateDto>(),
                    BankCities = new List<CityDto>()
                };

                try
                {
                    await LoadDropdownData(model);
                }
                catch (Exception ex)
                {
                    // Log the error but don't fail the page load
                    Console.WriteLine($"Error loading dropdown data: {ex.Message}");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Create GET: {ex}");
                TempData["ErrorMessage"] = "Error loading the form. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(CreateSchoolViewModel model)
		{
			if (!ModelState.IsValid)
			{
				await LoadDropdownData(model);
				return View(model);
			}

			try
			{
				var token = HttpContext.Session.GetString("JWToken") ?? string.Empty;
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var request = new CreateSchoolRequest
				{
					Name = model.Name,
					Description = model.Description,
					Email = model.Email,
					Address1 = model.Address1,
					Address2 = model.Address2,
					CityId = model.CityId,
					StateId = model.StateId,
					CountryId = model.CountryId,
					ZipCode = model.ZipCode,
					Phone = model.Phone,
					Mobile = model.Mobile,
					EstablishmentYear = model.EstablishmentYear,
					JudistrictionCityId = model.JudistrictionCityId,
					JudistrictionStateId = model.JudistrictionStateId,
					JudistrictionCountryId = model.JudistrictionCountryId,
					BankName = model.BankName,
					BankAddress1 = model.BankAddress1,
					BankAddress2 = model.BankAddress2,
					BankCityId = model.BankCityId,
					BankStateId = model.BankStateId,
					BankCountryId = model.BankCountryId,
					BankZipCode = model.BankZipCode,
					AccountNumber = model.AccountNumber,
					CompanyId = model.CompanyId,
					IsActive = model.IsActive
				};

				var response = await _httpClient.PostAsJsonAsync(_apiBaseUrl, request);
				response.EnsureSuccessStatusCode();

				TempData["SuccessMessage"] = "School created successfully!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception)
			{
				ModelState.AddModelError(string.Empty, "Error creating school. Please try again.");
				await LoadDropdownData(model);
				return View(model);
			}
		}

		[HttpGet]
		public async Task<IActionResult> Edit(Guid id)
		{
			try
			{
				var token = HttpContext.Session.GetString("JWToken") ?? string.Empty;
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				_logger.LogInformation("Loading school for edit. ID: {SchoolId}", id);
				var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{id}");
				response.EnsureSuccessStatusCode();

				var content = await response.Content.ReadAsStringAsync();
				var schoolDto = JsonSerializer.Deserialize<SchoolDto>(content, JsonOptions.Default);

				if (schoolDto == null)
				{
					_logger.LogWarning("School not found. ID: {SchoolId}", id);
					ModelState.AddModelError(string.Empty, "School not found.");
					return RedirectToAction(nameof(Index));
				}

				var model = new UpdateSchoolViewModel
				{
					Id = schoolDto.Id,
					Name = schoolDto.Name,
					Description = schoolDto.Description,
					Email = schoolDto.Email,
					Address1 = schoolDto.Address1,
					Address2 = schoolDto.Address2,
					CityId = schoolDto.CityId,
					StateId = schoolDto.StateId,
					CountryId = schoolDto.CountryId,
					ZipCode = schoolDto.ZipCode,
					Phone = schoolDto.Phone,
					Mobile = schoolDto.Mobile,
					EstablishmentYear = schoolDto.EstablishmentYear,
					JudistrictionCityId = schoolDto.JudistrictionCityId,
					JudistrictionStateId = schoolDto.JudistrictionStateId,
					JudistrictionCountryId = schoolDto.JudistrictionCountryId,
					BankName = schoolDto.BankName,
					BankAddress1 = schoolDto.BankAddress1,
					BankAddress2 = schoolDto.BankAddress2,
					BankCityId = schoolDto.BankCityId,
					BankStateId = schoolDto.BankStateId,
					BankCountryId = schoolDto.BankCountryId,
					BankZipCode = schoolDto.BankZipCode,
					AccountNumber = schoolDto.AccountNumber,
					CompanyId = schoolDto.CompanyId,
					IsActive = schoolDto.IsActive,
					Status = schoolDto.Status,
					StatusMessage = schoolDto.StatusMessage
				};

				await LoadDropdownData(model);
				return View(model);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error loading school data for edit. ID: {SchoolId}", id);
				ModelState.AddModelError(string.Empty, "Error loading school data. Please try again.");
				return RedirectToAction(nameof(Index));
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Guid id, UpdateSchoolViewModel model)
		{
			if (!ModelState.IsValid)
			{
				await LoadDropdownData(model);
				return View(model);
			}

			try
			{
				var token = HttpContext.Session.GetString("JWToken") ?? string.Empty;
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var request = new UpdateSchoolRequest
				{
					Id = model.Id,
					Name = model.Name,
					Description = model.Description,
					Email = model.Email,
					Address1 = model.Address1,
					Address2 = model.Address2,
					CityId = model.CityId,
					StateId = model.StateId,
					CountryId = model.CountryId,
					ZipCode = model.ZipCode,
					Phone = model.Phone,
					Mobile = model.Mobile,
					EstablishmentYear = model.EstablishmentYear,
					JudistrictionCityId = model.JudistrictionCityId,
					JudistrictionStateId = model.JudistrictionStateId,
					JudistrictionCountryId = model.JudistrictionCountryId,
					BankName = model.BankName,
					BankAddress1 = model.BankAddress1,
					BankAddress2 = model.BankAddress2,
					BankCityId = model.BankCityId,
					BankStateId = model.BankStateId,
					BankCountryId = model.BankCountryId,
					BankZipCode = model.BankZipCode,
					AccountNumber = model.AccountNumber,
					CompanyId = model.CompanyId,
					IsActive = model.IsActive,
					Status = model.Status,
					StatusMessage = model.StatusMessage
				};

				var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/{id}", request);
				response.EnsureSuccessStatusCode();

				TempData["SuccessMessage"] = "School updated successfully!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception)
			{
				ModelState.AddModelError(string.Empty, "Error updating school. Please try again.");
				await LoadDropdownData(model);
				return View(model);
			}
		}

		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(Guid id)
		{
			try
			{
				var token = HttpContext.Session.GetString("JWToken") ?? string.Empty;
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/{id}");
				response.EnsureSuccessStatusCode();

				TempData["SuccessMessage"] = "School deleted successfully!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception)
			{
				ModelState.AddModelError(string.Empty, "Error deleting school. Please try again.");
				return RedirectToAction(nameof(Index));
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetStatesByCountry(Guid countryId)
		{
			try
			{
				var token = HttpContext.Session.GetString("JWToken") ?? string.Empty;
				if (!string.IsNullOrWhiteSpace(token))
				{
					_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
				}

				_logger.LogInformation("Fetching states for country {CountryId}", countryId);
				var states = await GetCachedApiDataAsync<List<StateDto>>(
					$"States_Country_{countryId}",
					$"{_companyApiBaseUrl}/locations/states/{countryId}"
				) ?? new List<StateDto>();

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

				_logger.LogInformation("Fetching cities for state {StateId}", stateId);
				var cities = await GetCachedApiDataAsync<List<CityDto>>(
					$"Cities_State_{stateId}",
					$"{_companyApiBaseUrl}/locations/cities/{stateId}"
				) ?? new List<CityDto>();

				return Json(cities);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving cities for state {StateId}", stateId);
				return Json(new List<CityDto>());
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetCompanies()
		{
			try
			{
				var token = HttpContext.Session.GetString("JWToken") ?? string.Empty;
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				_logger.LogInformation("Fetching companies for dropdown");
				var companies = await GetCachedApiDataAsync<List<CompanyDto>>(
					"Companies_All",
					_companyApiBaseUrl
				) ?? new List<CompanyDto>();

				return Json(companies);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving companies for dropdown");
				return Json(new List<CompanyDto>());
			}
		}

		private async Task LoadDropdownData(SchoolBaseViewModel model)
		{
			try
			{
				var token = HttpContext.Session.GetString("JWToken") ?? string.Empty;
				if (!string.IsNullOrWhiteSpace(token))
				{
					_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
				}

<<<<<<< HEAD
				// Load companies
				Console.WriteLine("Loading companies...");
				var companiesResponse = await _httpClient.GetAsync(_companyApiBaseUrl);
				if (!companiesResponse.IsSuccessStatusCode)
				{
					var errorContent = await companiesResponse.Content.ReadAsStringAsync();
					throw new Exception($"Failed to load companies. Status: {companiesResponse.StatusCode}, Content: {errorContent}");
				}
				var companiesContent = await companiesResponse.Content.ReadAsStringAsync();
				model.Companies = JsonConvert.DeserializeObject<List<CompanyDto>>(companiesContent) ?? new List<CompanyDto>();
				Console.WriteLine($"Loaded {model.Companies.Count} companies");

				// Load countries
				Console.WriteLine("Loading countries...");
				var countriesResponse = await _httpClient.GetAsync($"{_companyApiBaseUrl}/locations/countries");
				if (!countriesResponse.IsSuccessStatusCode)
				{
					var errorContent = await countriesResponse.Content.ReadAsStringAsync();
					throw new Exception($"Failed to load countries. Status: {countriesResponse.StatusCode}, Content: {errorContent}");
				}
				var countriesContent = await countriesResponse.Content.ReadAsStringAsync();
				model.Countries = JsonConvert.DeserializeObject<List<CountryDto>>(countriesContent) ?? new List<CountryDto>();
				Console.WriteLine($"Loaded {model.Countries.Count} countries");

				// Set jurisdiction and bank countries to the same as regular countries
				model.JudistrictionCountries = model.Countries.ToList();
				model.BankCountries = model.Countries.ToList();
=======
				model.Companies = await GetCachedApiDataAsync<List<CompanyDto>>(
					"Companies_All",
					_companyApiBaseUrl
				) ?? new List<CompanyDto>();

				model.Countries = await GetCachedApiDataAsync<List<CountryDto>>(
					"Countries",
					$"{_companyApiBaseUrl}/locations/countries"
				) ?? new List<CountryDto>();
>>>>>>> 0e5f10e9c378fdd6a6a0df3d0162372e3d96a58f

				if (model.CountryId != Guid.Empty)
				{
<<<<<<< HEAD
					Console.WriteLine($"Loading states for country {model.CountryId}...");
					var statesResponse = await _httpClient.GetAsync($"{_companyApiBaseUrl}/locations/states/{model.CountryId}");
					if (statesResponse.IsSuccessStatusCode)
					{
						var statesContent = await statesResponse.Content.ReadAsStringAsync();
						model.States = JsonConvert.DeserializeObject<List<StateDto>>(statesContent) ?? new List<StateDto>();
						Console.WriteLine($"Loaded {model.States.Count} states");
					}
=======
					model.States = await GetCachedApiDataAsync<List<StateDto>>(
						$"States_Country_{model.CountryId}",
						$"{_companyApiBaseUrl}/locations/states/{model.CountryId}"
					) ?? new List<StateDto>();
>>>>>>> 0e5f10e9c378fdd6a6a0df3d0162372e3d96a58f
				}

				if (model.StateId != Guid.Empty)
				{
<<<<<<< HEAD
					Console.WriteLine($"Loading cities for state {model.StateId}...");
					var citiesResponse = await _httpClient.GetAsync($"{_companyApiBaseUrl}/locations/cities/{model.StateId}");
					if (citiesResponse.IsSuccessStatusCode)
					{
						var citiesContent = await citiesResponse.Content.ReadAsStringAsync();
						model.Cities = JsonConvert.DeserializeObject<List<CityDto>>(citiesContent) ?? new List<CityDto>();
						Console.WriteLine($"Loaded {model.Cities.Count} cities");
					}
				}

				// Load jurisdiction states if jurisdiction country is selected
				if (model.JudistrictionCountryId != Guid.Empty)
				{
					Console.WriteLine($"Loading jurisdiction states for country {model.JudistrictionCountryId}...");
					var jurStatesResponse = await _httpClient.GetAsync($"{_companyApiBaseUrl}/locations/states/{model.JudistrictionCountryId}");
					if (jurStatesResponse.IsSuccessStatusCode)
					{
						var jurStatesContent = await jurStatesResponse.Content.ReadAsStringAsync();
						model.JudistrictionStates = JsonConvert.DeserializeObject<List<StateDto>>(jurStatesContent) ?? new List<StateDto>();
						Console.WriteLine($"Loaded {model.JudistrictionStates.Count} jurisdiction states");
					}
=======
					model.Cities = await GetCachedApiDataAsync<List<CityDto>>(
						$"Cities_State_{model.StateId}",
						$"{_companyApiBaseUrl}/locations/cities/{model.StateId}"
					) ?? new List<CityDto>();
				}

				model.JudistrictionCountries = model.Countries;

				if (model.JudistrictionCountryId != Guid.Empty)
				{
					model.JudistrictionStates = await GetCachedApiDataAsync<List<StateDto>>(
						$"Jur_States_Country_{model.JudistrictionCountryId}",
						$"{_companyApiBaseUrl}/locations/states/{model.JudistrictionCountryId}"
					) ?? new List<StateDto>();
>>>>>>> 0e5f10e9c378fdd6a6a0df3d0162372e3d96a58f
				}

				if (model.JudistrictionStateId != Guid.Empty)
				{
<<<<<<< HEAD
					Console.WriteLine($"Loading jurisdiction cities for state {model.JudistrictionStateId}...");
					var jurCitiesResponse = await _httpClient.GetAsync($"{_companyApiBaseUrl}/locations/cities/{model.JudistrictionStateId}");
					if (jurCitiesResponse.IsSuccessStatusCode)
					{
						var jurCitiesContent = await jurCitiesResponse.Content.ReadAsStringAsync();
						model.JudistrictionCities = JsonConvert.DeserializeObject<List<CityDto>>(jurCitiesContent) ?? new List<CityDto>();
						Console.WriteLine($"Loaded {model.JudistrictionCities.Count} jurisdiction cities");
					}
				}

				// Load bank states if bank country is selected
				if (model.BankCountryId != Guid.Empty)
				{
					Console.WriteLine($"Loading bank states for country {model.BankCountryId}...");
					var bankStatesResponse = await _httpClient.GetAsync($"{_companyApiBaseUrl}/locations/states/{model.BankCountryId}");
					if (bankStatesResponse.IsSuccessStatusCode)
					{
						var bankStatesContent = await bankStatesResponse.Content.ReadAsStringAsync();
						model.BankStates = JsonConvert.DeserializeObject<List<StateDto>>(bankStatesContent) ?? new List<StateDto>();
						Console.WriteLine($"Loaded {model.BankStates.Count} bank states");
					}
=======
					model.JudistrictionCities = await GetCachedApiDataAsync<List<CityDto>>(
						$"Jur_Cities_State_{model.JudistrictionStateId}",
						$"{_companyApiBaseUrl}/locations/cities/{model.JudistrictionStateId}"
					) ?? new List<CityDto>();
				}

				model.BankCountries = model.Countries;

				if (model.BankCountryId != Guid.Empty)
				{
					model.BankStates = await GetCachedApiDataAsync<List<StateDto>>(
						$"Bank_States_Country_{model.BankCountryId}",
						$"{_companyApiBaseUrl}/locations/states/{model.BankCountryId}"
					) ?? new List<StateDto>();
>>>>>>> 0e5f10e9c378fdd6a6a0df3d0162372e3d96a58f
				}

				if (model.BankStateId != Guid.Empty)
				{
<<<<<<< HEAD
					Console.WriteLine($"Loading bank cities for state {model.BankStateId}...");
					var bankCitiesResponse = await _httpClient.GetAsync($"{_companyApiBaseUrl}/locations/cities/{model.BankStateId}");
					if (bankCitiesResponse.IsSuccessStatusCode)
					{
						var bankCitiesContent = await bankCitiesResponse.Content.ReadAsStringAsync();
						model.BankCities = JsonConvert.DeserializeObject<List<CityDto>>(bankCitiesContent) ?? new List<CityDto>();
						Console.WriteLine($"Loaded {model.BankCities.Count} bank cities");
					}
=======
					model.BankCities = await GetCachedApiDataAsync<List<CityDto>>(
						$"Bank_Cities_State_{model.BankStateId}",
						$"{_companyApiBaseUrl}/locations/cities/{model.BankStateId}"
					) ?? new List<CityDto>();
>>>>>>> 0e5f10e9c378fdd6a6a0df3d0162372e3d96a58f
				}
			}
			catch (Exception ex)
			{
<<<<<<< HEAD
				Console.WriteLine($"Error in LoadDropdownData: {ex}");
				
				// Initialize all collections to empty lists to prevent null reference exceptions
				model.Companies ??= new List<CompanyDto>();
				model.Countries ??= new List<CountryDto>();
				model.States ??= new List<StateDto>();
				model.Cities ??= new List<CityDto>();
				model.JudistrictionCountries ??= model.Countries.ToList();
				model.JudistrictionStates ??= new List<StateDto>();
				model.JudistrictionCities ??= new List<CityDto>();
				model.BankCountries ??= model.Countries.ToList();
				model.BankStates ??= new List<StateDto>();
				model.BankCities ??= new List<CityDto>();

				// Re-throw the exception to be handled by the calling method
				throw;
=======
				_logger.LogError(ex, "Error loading dropdown data");
				model.Companies = new List<CompanyDto>();
				model.Countries = new List<CountryDto>();
				model.States = new List<StateDto>();
				model.Cities = new List<CityDto>();
				model.JudistrictionCountries = new List<CountryDto>();
				model.JudistrictionStates = new List<StateDto>();
				model.JudistrictionCities = new List<CityDto>();
				model.BankCountries = new List<CountryDto>();
				model.BankStates = new List<StateDto>();
				model.BankCities = new List<CityDto>();
>>>>>>> 0e5f10e9c378fdd6a6a0df3d0162372e3d96a58f
			}
		}
	}
}
