using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SchoolPortal.API.DTOs.Role;
using SchoolPortal.API.Interfaces.Services;
using SchoolPortal.Web.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolPortal.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ILogger<RoleController> _logger;

        public RoleController(IRoleService roleService, ILogger<RoleController> logger)
        {
            _roleService = roleService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<API.DTOs.Role.RoleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var role = await _roleService.GetByIdAsync(id);
                if (role == null)
                {
                    return NotFound(ApiResponse.ErrorResult("Role not found"));
                }
                return Ok(ApiResponse<API.DTOs.Role.RoleDto>.SuccessResult(role));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role by ID: {RoleId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.ErrorResult("An error occurred while processing your request."));
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<API.DTOs.Role.RoleListDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var roles = await _roleService.GetAllAsync();
                return Ok(ApiResponse<IEnumerable<API.DTOs.Role.RoleListDto>>.SuccessResult(roles));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all roles");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.ErrorResult("An error occurred while processing your request."));
            }
        }

        [HttpGet("company/{companyId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<API.DTOs.Role.RoleListDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByCompanyId(Guid companyId)
        {
            try
            {
                var roles = await _roleService.GetByCompanyIdAsync(companyId);
                return Ok(ApiResponse<IEnumerable<API.DTOs.Role.RoleListDto>>.SuccessResult(roles));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting roles by company ID: {CompanyId}", companyId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.ErrorResult("An error occurred while processing your request."));
            }
        }

        [HttpGet("school/{schoolId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<API.DTOs.Role.RoleListDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBySchoolId(Guid schoolId)
        {
            try
            {
                var roles = await _roleService.GetBySchoolIdAsync(schoolId);
                return Ok(ApiResponse<IEnumerable<API.DTOs.Role.RoleListDto>>.SuccessResult(roles));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting roles by school ID: {SchoolId}", schoolId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.ErrorResult("An error occurred while processing your request."));
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<API.DTOs.Role.RoleDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] API.DTOs.Role.CreateRoleRequest request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var role = await _roleService.CreateAsync(request, currentUserId);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = role.Id },
                    ApiResponse<API.DTOs.Role.RoleDto>.SuccessResult(role, "Role created successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.ErrorResult("An error occurred while creating the role."));
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<API.DTOs.Role.RoleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] API.DTOs.Role.UpdateRoleRequest request)
        {
            try
            {
                if (id != request.Id)
                {
                    return BadRequest(ApiResponse.ErrorResult("ID in the URL does not match the request body."));
                }

                var currentUserId = GetCurrentUserId();
                var role = await _roleService.UpdateAsync(request, currentUserId);

                return Ok(ApiResponse<API.DTOs.Role.RoleDto>.SuccessResult(role, "Role updated successfully"));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse.ErrorResult("Role not found."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role with ID: {RoleId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.ErrorResult("An error occurred while updating the role."));
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                await _roleService.DeleteAsync(id, currentUserId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse.ErrorResult("Role not found."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role with ID: {RoleId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse.ErrorResult("An error occurred while deleting the role."));
            }
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("id");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user identifier");
            }
            return userId;
        }
    }
}