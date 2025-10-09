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
using SchoolPortal.API.DTOs.School;
using SchoolPortal.Web.Models.School;
using CompanyDto = SchoolPortal.API.DTOs.Company.CompanyDto;

namespace SchoolPortal.Web.Controllers
{
    [Authorize]
    public class SchoolController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;
        private readonly string _companyApiBaseUrl;

        public SchoolController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = _configuration["ApiSettings:BaseUrl"] + "/api/school";
            _companyApiBaseUrl = _configuration["ApiSettings:BaseUrl"] + "/api/company";
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
                var schools = JsonConvert.DeserializeObject<List<SchoolDto>>(content);

                return View(schools);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error retrieving schools. Please try again.";
                return View(new List<SchoolDto>());
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
                var school = JsonConvert.DeserializeObject<SchoolDto>(content);

                return View(school);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "School not found.";
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
                    ZipCode = string.Empty
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

                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{id}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var schoolDto = JsonConvert.DeserializeObject<SchoolDto>(content);

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
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"{_companyApiBaseUrl}/locations/states/{countryId}");
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
        public async Task<IActionResult> GetCitiesByState(Guid stateId)
        {
            try
            {
                var token = HttpContext.Session.GetString("JWToken") ?? string.Empty;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"{_companyApiBaseUrl}/locations/cities/{stateId}");
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

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            try
            {
                var token = HttpContext.Session.GetString("JWToken") ?? string.Empty;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync(_companyApiBaseUrl);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var companies = JsonConvert.DeserializeObject<List<CompanyDto>>(content);

                return Json(companies);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Error retrieving companies. Please try again.");
                return Json(new List<CompanyDto>());
            }
        }

        private async Task LoadDropdownData(SchoolBaseViewModel model)
        {
            try
            {
                var token = HttpContext.Session.GetString("JWToken") ?? string.Empty;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Load companies
                var companiesResponse = await _httpClient.GetAsync(_companyApiBaseUrl);
                companiesResponse.EnsureSuccessStatusCode();
                var companiesContent = await companiesResponse.Content.ReadAsStringAsync();
                model.Companies = JsonConvert.DeserializeObject<List<CompanyDto>>(companiesContent);

                // Load countries
                var countriesResponse = await _httpClient.GetAsync($"{_companyApiBaseUrl}/locations/countries");
                countriesResponse.EnsureSuccessStatusCode();
                var countriesContent = await countriesResponse.Content.ReadAsStringAsync();
                model.Countries = JsonConvert.DeserializeObject<List<CountryDto>>(countriesContent);

                // Load states if country is selected
                if (model.CountryId != Guid.Empty)
                {
                    var statesResponse = await _httpClient.GetAsync($"{_companyApiBaseUrl}/locations/states/{model.CountryId}");
                    statesResponse.EnsureSuccessStatusCode();
                    var statesContent = await statesResponse.Content.ReadAsStringAsync();
                    model.States = JsonConvert.DeserializeObject<List<StateDto>>(statesContent);
                }

                // Load cities if state is selected
                if (model.StateId != Guid.Empty)
                {
                    var citiesResponse = await _httpClient.GetAsync($"{_companyApiBaseUrl}/locations/cities/{model.StateId}");
                    citiesResponse.EnsureSuccessStatusCode();
                    var citiesContent = await citiesResponse.Content.ReadAsStringAsync();
                    model.Cities = JsonConvert.DeserializeObject<List<CityDto>>(citiesContent);
                }

                // Load jurisdiction countries (same as regular countries)
                model.JudistrictionCountries = model.Countries;

                // Load jurisdiction states if jurisdiction country is selected
                if (model.JudistrictionCountryId != Guid.Empty)
                {
                    var jurStatesResponse = await _httpClient.GetAsync($"{_companyApiBaseUrl}/locations/states/{model.JudistrictionCountryId}");
                    jurStatesResponse.EnsureSuccessStatusCode();
                    var jurStatesContent = await jurStatesResponse.Content.ReadAsStringAsync();
                    model.JudistrictionStates = JsonConvert.DeserializeObject<List<StateDto>>(jurStatesContent);
                }

                // Load jurisdiction cities if jurisdiction state is selected
                if (model.JudistrictionStateId != Guid.Empty)
                {
                    var jurCitiesResponse = await _httpClient.GetAsync($"{_companyApiBaseUrl}/locations/cities/{model.JudistrictionStateId}");
                    jurCitiesResponse.EnsureSuccessStatusCode();
                    var jurCitiesContent = await jurCitiesResponse.Content.ReadAsStringAsync();
                    model.JudistrictionCities = JsonConvert.DeserializeObject<List<CityDto>>(jurCitiesContent);
                }

                // Load bank countries (same as regular countries)
                model.BankCountries = model.Countries;

                // Load bank states if bank country is selected
                if (model.BankCountryId != Guid.Empty)
                {
                    var bankStatesResponse = await _httpClient.GetAsync($"{_companyApiBaseUrl}/locations/states/{model.BankCountryId}");
                    bankStatesResponse.EnsureSuccessStatusCode();
                    var bankStatesContent = await bankStatesResponse.Content.ReadAsStringAsync();
                    model.BankStates = JsonConvert.DeserializeObject<List<StateDto>>(bankStatesContent);
                }

                // Load bank cities if bank state is selected
                if (model.BankStateId != Guid.Empty)
                {
                    var bankCitiesResponse = await _httpClient.GetAsync($"{_companyApiBaseUrl}/locations/cities/{model.BankStateId}");
                    bankCitiesResponse.EnsureSuccessStatusCode();
                    var bankCitiesContent = await bankCitiesResponse.Content.ReadAsStringAsync();
                    model.BankCities = JsonConvert.DeserializeObject<List<CityDto>>(bankCitiesContent);
                }
            }
            catch (Exception)
            {
                // Handle error or log it
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
            }
        }
    }
}
