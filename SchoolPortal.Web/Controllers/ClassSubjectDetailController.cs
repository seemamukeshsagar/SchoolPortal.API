// SchoolPortal.Web/Controllers/ClassSubjectDetailController.cs
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SchoolPortal.API.DTOs.ClassSubjectDetail;
using SchoolPortal.API.Interfaces.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SchoolPortal.Web.Controllers
{
    [Authorize]
    public class ClassSubjectDetailController : Controller
    {
        private readonly IClassSubjectDetailService _classSubjectDetailService;
        private readonly ILogger<ClassSubjectDetailController> _logger;

        // Add these private fields to the controller
        private readonly IClassService _classService;
        private readonly ISubjectService _subjectService;

        public ClassSubjectDetailController(
            IClassSubjectDetailService classSubjectDetailService,
            IClassService classService,
            ISubjectService subjectService,
            ILogger<ClassSubjectDetailController> logger)
        {
            _classSubjectDetailService = classSubjectDetailService;
            _classService = classService;
            _subjectService = subjectService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var classSubjectDetails = await _classSubjectDetailService.GetAllClassSubjectDetailsAsync();
                return View(classSubjectDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading class subject details");
                TempData["ErrorMessage"] = "An error occurred while loading class subject details.";
                return View(Array.Empty<ClassSubjectDetailDto>());
            }
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        // Add this helper method
        private async Task PopulateDropdowns()
        {
            var classes = await _classService.GetAllClassesAsync();
            var subjects = await _subjectService.GetAllSubjectsAsync();

            ViewBag.Classes = new SelectList(classes, "Id", "ClassName");
            ViewBag.Subjects = new SelectList(subjects, "Id", "SubjectName");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateClassSubjectDetailRequest request)
        {
            if (!ModelState.IsValid)
            {
                 await PopulateDropdowns();
                 return View(request);
            }

            try
            {
                request.CreatedBy = Guid.Parse(HttpContext.Session.GetString("UserId"));
                await _classSubjectDetailService.CreateClassSubjectDetailAsync(request);
                TempData["SuccessMessage"] = "Class subject detail created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating class subject detail");
                ModelState.AddModelError("", "An error occurred while creating the class subject detail.");
                await PopulateDropdowns();
                return View(request);
            }
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var classSubjectDetail = await _classSubjectDetailService.GetClassSubjectDetailByIdAsync(id);
                var updateRequest = new UpdateClassSubjectDetailRequest
                {
                    Id = classSubjectDetail.Id,
                    IsActive = classSubjectDetail.IsActive,
                    Status = classSubjectDetail.Status,
                    StatusMessage = classSubjectDetail.StatusMessage
                };
                await PopulateDropdowns();
                return View(updateRequest);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading class subject detail for editing");
                TempData["ErrorMessage"] = "An error occurred while loading the class subject detail.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateClassSubjectDetailRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            try
            {
                request.ModifiedBy = Guid.Parse(HttpContext.Session.GetString("UserId"));
                await _classSubjectDetailService.UpdateClassSubjectDetailAsync(id, request);
                TempData["SuccessMessage"] = "Class subject detail updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating class subject detail");
                ModelState.AddModelError("", "An error occurred while updating the class subject detail.");
                return View(request);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _classSubjectDetailService.DeleteClassSubjectDetailAsync(id);
                TempData["SuccessMessage"] = "Class subject detail deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting class subject detail");
                TempData["ErrorMessage"] = "An error occurred while deleting the class subject detail.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}