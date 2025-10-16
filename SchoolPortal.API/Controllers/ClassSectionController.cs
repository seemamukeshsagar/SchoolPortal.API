// SchoolPortal.API/Controllers/ClassSectionController.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SchoolPortal.API.DTOs.ClassSection;
using SchoolPortal.API.Interfaces.Services;

namespace SchoolPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class ClassSectionController : ControllerBase
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

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ClassSectionDetailDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<ClassSectionDetailDto>>> GetAllClassSections()
        {
            try
            {
                var classSections = await _classSectionService.GetAllClassSectionsAsync();
                return Ok(classSections);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving class sections");
                return StatusCode(500, "An error occurred while retrieving class sections");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClassSectionDetailDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ClassSectionDetailDto>> GetClassSection(Guid id)
        {
            try
            {
                var classSection = await _classSectionService.GetClassSectionByIdAsync(id);
                if (classSection == null)
                {
                    return NotFound();
                }
                return Ok(classSection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving class section with ID: {ClassSectionId}", id);
                return StatusCode(500, "An error occurred while retrieving the class section");
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ClassSectionDetailDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ClassSectionDetailDto>> CreateClassSection(CreateClassSectionDetailRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var classSection = await _classSectionService.CreateClassSectionAsync(request);
                return CreatedAtAction(nameof(GetClassSection), new { id = classSection.Id }, classSection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating class section");
                return StatusCode(500, "An error occurred while creating the class section");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ClassSectionDetailDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateClassSection(Guid id, UpdateClassSectionDetailRequest request)
        {
            try
            {
                if (id != request.Id)
                {
                    return BadRequest("ID in URL does not match ID in request body");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var classSection = await _classSectionService.UpdateClassSectionAsync(request);
                if (classSection == null)
                {
                    return NotFound();
                }

                return Ok(classSection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating class section with ID: {ClassSectionId}", id);
                return StatusCode(500, "An error occurred while updating the class section");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteClassSection(Guid id)
        {
            try
            {
                var result = await _classSectionService.DeleteClassSectionAsync(id);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting class section with ID: {ClassSectionId}", id);
                return StatusCode(500, "An error occurred while deleting the class section");
            }
        }

        [HttpGet("class/{classId}")]
        [ProducesResponseType(typeof(IEnumerable<ClassSectionDetailDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<ClassSectionDetailDto>>> GetClassSectionsByClassId(Guid classId)
        {
            try
            {
                var classSections = await _classSectionService.GetClassSectionsByClassIdAsync(classId);
                return Ok(classSections);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving class sections for class ID: {ClassId}", classId);
                return StatusCode(500, "An error occurred while retrieving class sections for the specified class");
            }
        }

        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<ClassSectionDetailDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<ClassSectionDetailDto>>> GetActiveClassSections()
        {
            try
            {
                var classSections = await _classSectionService.GetActiveClassSectionsAsync();
                return Ok(classSections);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active class sections");
                return StatusCode(500, "An error occurred while retrieving active class sections");
            }
        }
    }
}