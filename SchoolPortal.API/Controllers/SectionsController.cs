using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SchoolPortal.API.DTOs.Section;
using SchoolPortal.API.Interfaces.Services;

namespace SchoolPortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SectionsController : ControllerBase
    {
        private readonly ILogger<SectionsController> _logger;
        private readonly ISectionService _sectionService;

        public SectionsController(ILogger<SectionsController> logger, ISectionService sectionService)
        {
            _logger = logger;
            _sectionService = sectionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SectionListDto>>> GetSections()
        {
            try
            {
                var sections = await _sectionService.GetAllSectionsAsync();
                return Ok(sections);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching sections");
                return StatusCode(500, "Internal server error while fetching sections");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SectionDto>> GetSection(Guid id)
        {
            try
            {
                var section = await _sectionService.GetSectionByIdAsync(id);
                if (section == null)
                {
                    return NotFound();
                }
                return Ok(section);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching section with id: {id}");
                return StatusCode(500, "Internal server error while fetching section");
            }
        }

        [HttpPost]
        public async Task<ActionResult<SectionDto>> CreateSection(CreateSectionRequest request)
        {
            try
            {
                var result = await _sectionService.CreateSectionAsync(request);
                return CreatedAtAction(nameof(GetSection), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating section");
                return StatusCode(500, "Internal server error while creating section");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSection(Guid id, UpdateSectionRequest request)
        {
            try
            {
                var result = await _sectionService.UpdateSectionAsync(id, request);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating section with id: {id}");
                return StatusCode(500, "Internal server error while updating section");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSection(Guid id)
        {
            try
            {
                var result = await _sectionService.DeleteSectionAsync(id);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting section with id: {id}");
                return StatusCode(500, "Internal server error while deleting section");
            }
        }
    }
}