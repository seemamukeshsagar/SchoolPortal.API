using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SchoolPortal.API.DTOs;
using SchoolPortal.API.DTOs.Company;
using SchoolPortal.Web.Models.Company;

namespace SchoolPortal.Web.Controllers
{
	// [Authorize]
	public class CompanyController : Controller
	{
		private readonly HttpClient _httpClient;
		private readonly IConfiguration _configuration;
		private readonly string _apiBaseUrl;

		public CompanyController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
		{
			_configuration = configuration;
			_httpClient = httpClientFactory.CreateClient();
			_apiBaseUrl = _configuration["ApiSettings:BaseUrl"] + "/api/company";
		}
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			try
			{
				var token = HttpContext.Session.GetString("JWToken") ?? string.Empty;
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await _httpClient.GetAsync(_apiBaseUrl);
				response.EnsureSuccessStatusCode();

				var content = await response.Content.ReadAsStringAsync();
				var companies = JsonConvert.DeserializeObject<List<API.DTOs.CompanyDto>>(content);

				return View(companies);
			}
			catch (Exception)
			{
				TempData["ErrorMessage"] = "Error retrieving companies. Please try again.";
				return View(new List<API.DTOs.CompanyDto>());
			}
		}	

		[HttpGet]
		public async Task<IActionResult> Details(Guid id)
		{
			try
			{
				var token = HttpContext.Session.GetString("JWToken") ?? string.Empty;
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{id}");
				response.EnsureSuccessStatusCode();

				var content = await response.Content.ReadAsStringAsync();
				var company = JsonConvert.DeserializeObject<API.DTOs.CompanyDto>(content);

				return View(company);
			}
			catch (Exception)
			{
				TempData["ErrorMessage"] = "Company not found.";
				return RedirectToAction(nameof(Index));
			}
		}

		[HttpGet]
		public async Task<IActionResult> Create()
		{
			try
			{
				var token = HttpContext.Session.GetString("JWToken") ?? string.Empty;
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var model = new CreateCompanyViewModel
				{
					CompanyName = string.Empty,
					Description = string.Empty,
					Address = string.Empty,
					ZipCode = string.Empty,
					Email = string.Empty,
					EstablishmentYear = string.Empty
				};
				await LoadDropdownData(model);
				return View(model);
			}
			catch (Exception)
			{
				TempData["ErrorMessage"] = "Error loading form. Please try again.";
				return RedirectToAction(nameof(Index));
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(CreateCompanyViewModel model)
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

				var response = await _httpClient.PostAsJsonAsync(_apiBaseUrl, model);
				response.EnsureSuccessStatusCode();

				TempData["SuccessMessage"] = "Company created successfully!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception)
			{
				ModelState.AddModelError(string.Empty, "Error creating company. Please try again.");
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

				var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{id}");
				response.EnsureSuccessStatusCode();

				var content = await response.Content.ReadAsStringAsync();
				var model = JsonConvert.DeserializeObject<UpdateCompanyViewModel>(content);

				await LoadDropdownData(model);
				return View(model);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, "Error loading company data. Please try again.");
				return RedirectToAction(nameof(Index));
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Guid id, UpdateCompanyViewModel model)
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

				var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/{id}", model);
				response.EnsureSuccessStatusCode();

				TempData["SuccessMessage"] = "Company updated successfully!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception)
			{
				ModelState.AddModelError(string.Empty, "Error updating company. Please try again.");
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

				TempData["SuccessMessage"] = "Company deleted successfully!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception)
			{
				ModelState.AddModelError(string.Empty, "Error deleting company. Please try again.");
				return View();
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetStatesByCountry(Guid countryId)
		{
			try
			{
				var token = HttpContext.Session.GetString("JWToken") ?? string.Empty;
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await _httpClient.GetAsync($"{_apiBaseUrl}/locations/states/{countryId}");
				response.EnsureSuccessStatusCode();

				var content = await response.Content.ReadAsStringAsync();
				var states = JsonConvert.DeserializeObject<List<StateDto>>(content);

				return Json(states);
			}
			catch (Exception)
			{
				ModelState.AddModelError(string.Empty, "Error retrieving states. Please try again.");
				return Json(new List<StateDto>());
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetCityByState(Guid stateId)
		{
			try
			{
				var token = HttpContext.Session.GetString("JWToken") ?? string.Empty;
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await _httpClient.GetAsync($"{_apiBaseUrl}/locations/cities/{stateId}");
				response.EnsureSuccessStatusCode();

				var content = await response.Content.ReadAsStringAsync();
				var cities = JsonConvert.DeserializeObject<List<CityDto>>(content);

				return Json(cities);
			}
			catch (Exception)
			{
				ModelState.AddModelError(string.Empty, "Error retrieving cities. Please try again.");
				return Json(new List<CityDto>());
			}
		}
		
		private async Task LoadDropdownData(CompanyBaseViewModel model)
		{
			try
			{
				var token = HttpContext.Session.GetString("JWToken") ?? string.Empty;
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

				var response = await _httpClient.GetAsync($"{_apiBaseUrl}/locations/countries");
				response.EnsureSuccessStatusCode();

				var content = await response.Content.ReadAsStringAsync();
				model.Countries = JsonConvert.DeserializeObject<List<CountryDto>>(content);

				if (model.CountryId != Guid.Empty)
				{
					response = await _httpClient.GetAsync($"{_apiBaseUrl}/locations/states/{model.CountryId}");
					response.EnsureSuccessStatusCode();
					var statesContent = await response.Content.ReadAsStringAsync();
					model.States = JsonConvert.DeserializeObject<List<StateDto>>(statesContent);
				}

				if (model.StateId != Guid.Empty)
				{
					response = await _httpClient.GetAsync($"{_apiBaseUrl}/locations/cities/{model.StateId}");
					response.EnsureSuccessStatusCode();
					var citiesContent = await response.Content.ReadAsStringAsync();
					model.Cities = JsonConvert.DeserializeObject<List<CityDto>>(citiesContent);
				}

				// Set Judistriction Area options to be the same as Cities
				model.JudistrictionAreas = model.Cities;
			}
			catch (Exception)
			{
				// Handle error or log it
				model.Countries = new List<CountryDto>();
				model.States = new List<StateDto>();
				model.Cities = new List<CityDto>();
				model.JudistrictionAreas = new List<CityDto>();
			}
		}
	}
}