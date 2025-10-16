using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SchoolPortal.API.DTOs.Section;
using SchoolPortal.Web.Models.Section;

namespace SchoolPortal.Web.Controllers
{
    [Authorize]
    public class SectionController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly ILogger<SectionController> _logger;

        public SectionController(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ILogger<SectionController> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            var baseUrl = configuration["ApiSettings:BaseUrl"] ?? 
                throw new ArgumentNullException("ApiSettings:BaseUrl configuration is missing.");
            _apiBaseUrl = $"{baseUrl.TrimEnd('/')}/sections";
            _logger = logger;
        }

        private void SetAuthorizationHeader()
        {
            var token = HttpContext.Session.GetString("JWToken") ?? string.Empty;
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<IActionResult> Index()
        {
            SetAuthorizationHeader();
            
            try
            {
                var response = await _httpClient.GetAsync(_apiBaseUrl);
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                var sections = JsonSerializer.Deserialize<List<SectionListDto>>(
                    content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                return View(sections ?? new List<SectionListDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching sections");
                ViewBag.ErrorMessage = "Error loading sections. Please try again later.";
                return View(new List<SectionListDto>());
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            SetAuthorizationHeader();
            
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var content = await response.Content.ReadAsStringAsync();
                var section = JsonSerializer.Deserialize<SectionDto>(
                    content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (section == null)
                {
                    return NotFound();
                }

                return View(section);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching section with id: {id}");
                return StatusCode(500, "Error loading section details");
            }
        }

        public IActionResult Create()
        {
            return View(new CreateSectionViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSectionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            SetAuthorizationHeader();
            
            try
            {
                var request = new CreateSectionRequest
                {
                    Name = model.Name,
                    CompanyId = model.CompanyId,
                    SchoolId = model.SchoolId,
                    IsActive = model.IsActive
                };

                var response = await _httpClient.PostAsJsonAsync(_apiBaseUrl, request);
                response.EnsureSuccessStatusCode();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating section");
                ModelState.AddModelError("", "Error creating section. Please try again.");
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            SetAuthorizationHeader();
            
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var content = await response.Content.ReadAsStringAsync();
                var section = JsonSerializer.Deserialize<SectionDto>(
                    content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (section == null)
                {
                    return NotFound();
                }

                var model = new EditSectionViewModel
                {
                    Id = section.Id,
                    Name = section.Name,
                    CompanyId = section.CompanyId,
                    SchoolId = section.SchoolId,
                    IsActive = section.IsActive
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading section for edit with id: {id}");
                return StatusCode(500, "Error loading section for editing");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditSectionViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            SetAuthorizationHeader();
            
            try
            {
                var request = new UpdateSectionRequest
                {
                    Name = model.Name,
                    CompanyId = model.CompanyId,
                    SchoolId = model.SchoolId,
                    IsActive = model.IsActive
                };

                var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/{id}", request);
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound();
                }
                
                response.EnsureSuccessStatusCode();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating section with id: {id}");
                ModelState.AddModelError("", "Error updating section. Please try again.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            SetAuthorizationHeader();
            
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/{id}");
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound();
                }
                
                response.EnsureSuccessStatusCode();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting section with id: {id}");
                return StatusCode(500, "Error deleting section");
            }
        }
    }
}