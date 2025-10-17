using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SchoolPortal.API.DTOs.Subject;
using SchoolPortal.API.Interfaces.Services;

namespace SchoolPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;
        private readonly ILogger<SubjectController> _logger;

        public SubjectController(ISubjectService subjectService, ILogger<SubjectController> logger)
        {
            _subjectService = subjectService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SubjectDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SubjectDto>>> GetAllSubjects()
        {
            try
            {
                var subjects = await _subjectService.GetAllSubjectsAsync();
                return Ok(subjects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subjects");
                return StatusCode(500, new { message = "An error occurred while retrieving subjects" });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SubjectDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<SubjectDto>> GetSubject(Guid id)
        {
            try
            {
                var subject = await _subjectService.GetSubjectByIdAsync(id);
                return Ok(subject);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Subject with ID {id} not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subject with ID: {SubjectId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the subject" });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(SubjectDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<SubjectDto>> CreateSubject([FromBody] CreateSubjectRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { 
                        message = "Invalid request data", 
                        errors = ModelState.Values.SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                    });
                }

                var subject = await _subjectService.CreateSubjectAsync(request);
                return CreatedAtAction(nameof(GetSubject), new { id = subject.Id }, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subject");
                return StatusCode(500, new { message = "An error occurred while creating the subject" });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SubjectDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<SubjectDto>> UpdateSubject(Guid id, [FromBody] UpdateSubjectRequest request)
        {
            try
            {
                if (id != request.Id)
                {
                    return BadRequest(new { message = "ID in the URL does not match the ID in the request body" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { 
                        message = "Invalid request data", 
                        errors = ModelState.Values.SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                    });
                }

                var subject = await _subjectService.UpdateSubjectAsync(request);
                return Ok(subject);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Subject not found with ID: {SubjectId}", id);
                return NotFound(new { message = $"Subject with ID {id} not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subject with ID: {SubjectId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the subject" });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteSubject(Guid id)
        {
            try
            {
                var result = await _subjectService.DeleteSubjectAsync(id);
                if (!result)
                {
                    return NotFound(new { message = $"Subject with ID {id} not found" });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting subject with ID: {SubjectId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the subject" });
            }
        }

        [HttpGet("school/{schoolId}")]
        [ProducesResponseType(typeof(IEnumerable<SubjectDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SubjectDto>>> GetSubjectsBySchool(Guid schoolId)
        {
            try
            {
                var subjects = await _subjectService.GetSubjectsBySchoolAsync(schoolId);
                return Ok(subjects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subjects for school ID: {SchoolId}", schoolId);
                return StatusCode(500, new { message = "An error occurred while retrieving subjects for the school" });
            }
        }
    }
}