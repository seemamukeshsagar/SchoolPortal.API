// Controllers/SubjectController.cs
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SchoolPortal.API.DTOs.Subject;
using SchoolPortal.API.Interfaces.Services;

namespace SchoolPortal.Web.Controllers
{
    public class SubjectController : Controller
    {
        private readonly ISubjectService _subjectService;
        private readonly ILogger<SubjectController> _logger;

        public SubjectController(ISubjectService subjectService, ILogger<SubjectController> logger)
        {
            _subjectService = subjectService;
            _logger = logger;
        }

        // GET: Subject
        public async Task<IActionResult> Index()
        {
            try
            {
                var subjects = await _subjectService.GetAllSubjectsAsync();
                return View(subjects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subjects");
                TempData["ErrorMessage"] = "An error occurred while retrieving subjects";
                return View(Array.Empty<SubjectDto>());
            }
        }

        // GET: Subject/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var subject = await _subjectService.GetSubjectByIdAsync(id);
                if (subject == null)
                {
                    return NotFound();
                }
                return View(subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subject details for ID: {SubjectId}", id);
                TempData["ErrorMessage"] = "An error occurred while retrieving subject details";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Subject/Create
        public IActionResult Create()
        {
            return View(new CreateSubjectRequest());
        }

        // POST: Subject/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSubjectRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            try
            {
                await _subjectService.CreateSubjectAsync(request);
                TempData["SuccessMessage"] = "Subject created successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subject");
                ModelState.AddModelError("", "An error occurred while creating the subject");
                return View(request);
            }
        }

        // GET: Subject/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var subject = await _subjectService.GetSubjectByIdAsync(id);
                if (subject == null)
                {
                    return NotFound();
                }

                var updateRequest = new UpdateSubjectRequest
                {
                    Id = subject.Id,
                    SubjectName = subject.SubjectName,
                    CompanyId = subject.CompanyId,
                    SchoolId = subject.SchoolId,
                    IsScholastic = subject.IsScholastic,
                    IsActive = subject.IsActive,
                    Status = subject.Status,
                    StatusMessage = subject.StatusMessage,
                    ModifiedBy = GetCurrentUserId() // You'll need to implement this
                };

                return View(updateRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subject for edit with ID: {SubjectId}", id);
                TempData["ErrorMessage"] = "An error occurred while retrieving the subject for editing";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Subject/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateSubjectRequest request)
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
                await _subjectService.UpdateSubjectAsync(request);
                TempData["SuccessMessage"] = "Subject updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subject with ID: {SubjectId}", id);
                ModelState.AddModelError("", "An error occurred while updating the subject");
                return View(request);
            }
        }

        // GET: Subject/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var subject = await _subjectService.GetSubjectByIdAsync(id);
                if (subject == null)
                {
                    return NotFound();
                }
                return View(subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subject for deletion with ID: {SubjectId}", id);
                TempData["ErrorMessage"] = "An error occurred while retrieving the subject for deletion";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Subject/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var result = await _subjectService.DeleteSubjectAsync(id);
                if (!result)
                {
                    return NotFound();
                }
                TempData["SuccessMessage"] = "Subject deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting subject with ID: {SubjectId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the subject";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        private Guid GetCurrentUserId()
        {
            // Implement your logic to get the current user's ID
            // This is just a placeholder - replace with your actual implementation
            return Guid.Parse(User.FindFirst("sub")?.Value ?? Guid.Empty.ToString());
        }
    }
}