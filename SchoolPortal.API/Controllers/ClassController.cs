// Controllers/ClassController.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SchoolPortal.API.DTOs.Class;
using SchoolPortal.API.Interfaces.Services;

namespace SchoolPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClassController : ControllerBase
    {
        private readonly IClassService _classService;
        private readonly ILogger<ClassController> _logger;

        public ClassController(
            IClassService classService,
            ILogger<ClassController> logger)
        {
            _classService = classService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClassListDto>>> GetClasses()
        {
            try
            {
                var classes = await _classService.GetAllClassesAsync();
                return Ok(classes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all classes");
                return StatusCode(500, "Internal server error while retrieving classes");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClassDto>> GetClass(Guid id)
        {
            try
            {
                var classDto = await _classService.GetClassByIdAsync(id);
                if (classDto == null)
                {
                    return NotFound($"Class with ID {id} not found");
                }
                return Ok(classDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting class with ID {ClassId}", id);
                return StatusCode(500, $"Internal server error while retrieving class with ID {id}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ClassDto>> CreateClass(CreateClassRequest request)
        {
            try
            {
                // Get the current user's ID from the claims
                var userId = User.FindFirst("uid")?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userIdGuid))
                {
                    return Unauthorized("Invalid user");
                }

                var classDto = await _classService.CreateClassAsync(request, userIdGuid);
                return CreatedAtAction(nameof(GetClass), new { id = classDto.Id }, classDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating class");
                return StatusCode(500, "Internal server error while creating class");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClass(Guid id, UpdateClassRequest request)
        {
            try
            {
                var userId = User.FindFirst("uid")?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userIdGuid))
                {
                    return Unauthorized("Invalid user");
                }

                var classDto = await _classService.UpdateClassAsync(id, request, userIdGuid);
                if (classDto == null)
                {
                    return NotFound($"Class with ID {id} not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating class with ID {ClassId}", id);
                return StatusCode(500, $"Internal server error while updating class with ID {id}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClass(Guid id)
        {
            try
            {
                var userId = User.FindFirst("uid")?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userIdGuid))
                {
                    return Unauthorized("Invalid user");
                }

                var result = await _classService.DeleteClassAsync(id, userIdGuid);
                if (!result)
                {
                    return NotFound($"Class with ID {id} not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting class with ID {ClassId}", id);
                return StatusCode(500, $"Internal server error while deleting class with ID {id}");
            }
        }
    }
}