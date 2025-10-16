// Controllers/ClassController.cs
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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SchoolPortal.API.DTOs.Class;
using SchoolPortal.Web.Models.Class;
using SchoolPortal.Web.Common;

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
    public class ClassController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly ILogger<ClassController> _logger;

        public ClassController(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ILogger<ClassController> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration["ApiBaseUrl"] ?? throw new ArgumentNullException("ApiBaseUrl is not configured");
            _logger = logger;
        }

        // GET: Class
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/class");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var classes = JsonSerializer.Deserialize<List<ClassListDto>>(content, JsonOptions.Default);
                
                var viewModel = classes.Select(c => new ClassListViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    ExamAssessment = c.ExamAssessment,
                    IsActive = c.IsActive,
                    OrderBy = c.OrderBy,
                    SchoolName = c.SchoolName,
                    CompanyName = c.CompanyName
                }).ToList();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving classes");
                TempData["ErrorMessage"] = "An error occurred while retrieving classes.";
                return View(new List<ClassListViewModel>());
            }
        }

        // GET: Class/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/class/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return NotFound();
                    }
                    throw new Exception($"API returned status code {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var classDto = JsonSerializer.Deserialize<ClassDto>(content, JsonOptions.Default);
                
                var viewModel = new ClassViewModel
                {
                    Id = classDto.Id,
                    Name = classDto.Name,
                    ExamAssessment = classDto.ExamAssessment,
                    IsGradePointApplicable = classDto.IsGradePointApplicable,
                    IsActive = classDto.IsActive,
                    CompanyId = classDto.CompanyId,
                    SchoolId = classDto.SchoolId,
                    OrderBy = classDto.OrderBy,
                    Status = classDto.Status,
                    StatusMessage = classDto.StatusMessage
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving class with ID {id}");
                TempData["ErrorMessage"] = "An error occurred while retrieving the class details.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Class/Create
        public IActionResult Create()
        {
            return View(new ClassViewModel());
        }

        // POST: Class/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClassViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var createRequest = new CreateClassRequest
                {
                    Name = model.Name,
                    ExamAssessment = model.ExamAssessment,
                    IsGradePointApplicable = model.IsGradePointApplicable,
                    IsActive = model.IsActive,
                    CompanyId = model.CompanyId,
                    SchoolId = model.SchoolId,
                    OrderBy = model.OrderBy
                };

                var response = await _httpClient.PostAsJsonAsync(
                    $"{_apiBaseUrl}/api/class", 
                    createRequest,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"API Error: {response.StatusCode} - {errorContent}");
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the class.");
                    return View(model);
                }

                TempData["SuccessMessage"] = "Class created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating class");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the class. Please try again.");
                return View(model);
            }
        }

        // GET: Class/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/class/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return NotFound();
                    }
                    throw new Exception($"API returned status code {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var classDto = JsonSerializer.Deserialize<ClassDto>(content, JsonOptions.Default);
                
                var viewModel = new ClassViewModel
                {
                    Id = classDto.Id,
                    Name = classDto.Name,
                    ExamAssessment = classDto.ExamAssessment,
                    IsGradePointApplicable = classDto.IsGradePointApplicable,
                    IsActive = classDto.IsActive,
                    CompanyId = classDto.CompanyId,
                    SchoolId = classDto.SchoolId,
                    OrderBy = classDto.OrderBy
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving class with ID {id} for edit");
                TempData["ErrorMessage"] = "An error occurred while retrieving the class for editing.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Class/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ClassViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var updateRequest = new UpdateClassRequest
                {
                    Name = model.Name,
                    ExamAssessment = model.ExamAssessment,
                    IsGradePointApplicable = model.IsGradePointApplicable,
                    IsActive = model.IsActive,
                    OrderBy = model.OrderBy
                };

                var response = await _httpClient.PutAsJsonAsync(
                    $"{_apiBaseUrl}/api/class/{id}",
                    updateRequest,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return NotFound();
                    }

                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"API Error: {response.StatusCode} - {errorContent}");
                    ModelState.AddModelError(string.Empty, "An error occurred while updating the class.");
                    return View(model);
                }

                TempData["SuccessMessage"] = "Class updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating class with ID {id}");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the class. Please try again.");
                return View(model);
            }
        }

        // POST: Class/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/api/class/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return NotFound();
                    }

                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"API Error: {response.StatusCode} - {errorContent}");
                    TempData["ErrorMessage"] = "An error occurred while deleting the class.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = "Class deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting class with ID {id}");
                TempData["ErrorMessage"] = "An error occurred while deleting the class. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}