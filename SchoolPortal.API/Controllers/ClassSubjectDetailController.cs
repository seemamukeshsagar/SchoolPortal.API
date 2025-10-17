// SchoolPortal.API/Controllers/ClassSubjectDetailController.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SchoolPortal.API.DTOs.ClassSubjectDetail;
using SchoolPortal.API.Interfaces.Services;

namespace SchoolPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ClassSubjectDetailController : ControllerBase
    {
        private readonly IClassSubjectDetailService _classSubjectDetailService;
        private readonly ILogger<ClassSubjectDetailController> _logger;

        public ClassSubjectDetailController(IClassSubjectDetailService classSubjectDetailService, ILogger<ClassSubjectDetailController> logger)
        {
            _classSubjectDetailService = classSubjectDetailService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ClassSubjectDetailDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<ClassSubjectDetailDto>>> GetAllClassSubjectDetails()
        {
            try
            {
                var classSubjectDetails = await _classSubjectDetailService.GetAllClassSubjectDetailsAsync();
                return Ok(classSubjectDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving class subject details");
                return StatusCode(500, new { message = "An error occurred while retrieving class subject details" });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClassSubjectDetailDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ClassSubjectDetailDto>> GetClassSubjectDetail(Guid id)
        {
            try
            {
                var classSubjectDetail = await _classSubjectDetailService.GetClassSubjectDetailByIdAsync(id);
                return Ok(classSubjectDetail);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"ClassSubjectDetail with ID {id} not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving class subject detail with ID: {ClassSubjectDetailId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the class subject detail" });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ClassSubjectDetailDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ClassSubjectDetailDto>> CreateClassSubjectDetail([FromBody] CreateClassSubjectDetailRequest request)
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

                var classSubjectDetail = await _classSubjectDetailService.CreateClassSubjectDetailAsync(request);
                return CreatedAtAction(nameof(GetClassSubjectDetail), new { id = classSubjectDetail.Id }, classSubjectDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating class subject detail");
                return StatusCode(500, new { message = "An error occurred while creating the class subject detail" });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ClassSubjectDetailDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ClassSubjectDetailDto>> UpdateClassSubjectDetail(Guid id, [FromBody] UpdateClassSubjectDetailRequest request)
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

                var classSubjectDetail = await _classSubjectDetailService.UpdateClassSubjectDetailAsync(id, request);
                return Ok(classSubjectDetail);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"ClassSubjectDetail with ID {id} not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating class subject detail with ID: {ClassSubjectDetailId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the class subject detail" });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteClassSubjectDetail(Guid id)
        {
            try
            {
                await _classSubjectDetailService.DeleteClassSubjectDetailAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"ClassSubjectDetail with ID {id} not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting class subject detail with ID: {ClassSubjectDetailId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the class subject detail" });
            }
        }
    }
}