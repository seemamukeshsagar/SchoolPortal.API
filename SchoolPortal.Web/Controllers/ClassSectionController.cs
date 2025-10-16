// SchoolPortal.Web/Controllers/ClassSectionController.cs
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SchoolPortal.API.DTOs.ClassSection;
using SchoolPortal.API.Interfaces.Services;

namespace SchoolPortal.Web.Controllers
{
    [Authorize]
    public class ClassSectionController : Controller
    {
        private readonly IClassSectionService _classSectionService;
        private readonly ILogger<ClassSectionController> _logger;

        public ClassSectionController(
            IClassSectionService classSectionService,
            ILogger<ClassSectionController> logger)
        {
            _classSectionService = classSectionService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var classSections = await _classSectionService.GetAllClassSectionsAsync();
                return View(classSections);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading class sections");
                TempData["ErrorMessage"] = "An error occurred while loading class sections.";
                return View(Array.Empty<ClassSectionDetailDto>());
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var classSection = await _classSectionService.GetClassSectionByIdAsync(id);
                if (classSection == null)
                {
                    return NotFound();
                }
                return View(classSection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving class section with ID: {ClassSectionId}", id);
                TempData["ErrorMessage"] = "An error occurred while retrieving the class section details.";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateClassSectionDetailRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(request);
                }

                var result = await _classSectionService.CreateClassSectionAsync(request);
                if (result == null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to create class section.");
                    return View(request);
                }

                TempData["SuccessMessage"] = "Class section created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating class section");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the class section.");
                return View(request);
            }
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var classSection = await _classSectionService.GetClassSectionByIdAsync(id);
                if (classSection == null)
                {
                    return NotFound();
                }

                var model = new UpdateClassSectionDetailRequest
                {
                    Id = classSection.Id,
                    ClassId = classSection.ClassId,
                    SectionId = classSection.SectionId,
                    ClassTeacherId = classSection.ClassTeacherId,
                    MaxStudents = classSection.MaxStudents,
                    IsActive = classSection.IsActive,
                    AcademicYear = classSection.AcademicYear,
                    Remarks = classSection.Remarks
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading class section for edit with ID: {ClassSectionId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading the class section for editing.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateClassSectionDetailRequest request)
        {
            if (id != request.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(request);
            }

            try
            {
                var result = await _classSectionService.UpdateClassSectionAsync(request);
                if (result == null)
                {
                    return NotFound();
                }

                TempData["SuccessMessage"] = "Class section updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating class section with ID: {ClassSectionId}", id);
                ModelState.AddModelError(string.Empty, "An error occurred while updating the class section.");
                return View(request);
            }
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var classSection = await _classSectionService.GetClassSectionByIdAsync(id);
                if (classSection == null)
                {
                    return NotFound();
                }
                return View(classSection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading class section for deletion with ID: {ClassSectionId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading the class section for deletion.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var result = await _classSectionService.DeleteClassSectionAsync(id);
                if (!result)
                {
                    return NotFound();
                }

                TempData["SuccessMessage"] = "Class section deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting class section with ID: {ClassSectionId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the class section.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> GetByClass(Guid classId)
        {
            try
            {
                var classSections = await _classSectionService.GetClassSectionsByClassIdAsync(classId);
                return Json(classSections);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving class sections for class ID: {ClassId}", classId);
                return Json(new { success = false, message = "An error occurred while retrieving class sections." });
            }
        }

        public async Task<IActionResult> GetActiveSections()
        {
            try
            {
                var classSections = await _classSectionService.GetActiveClassSectionsAsync();
                return Json(classSections);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active class sections");
                return Json(new { success = false, message = "An error occurred while retrieving active class sections." });
            }
        }
    }
}