using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SchoolPortal.API.DTOs.School;
using SchoolPortal.API.Interfaces.Services;
using SchoolDto = SchoolPortal.API.DTOs.School.SchoolDto;

namespace SchoolPortal.API.Controllers
{
	/// <summary>
	/// Controller for managing school operations
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	// [Authorize]
	[Produces("application/json")]
	public class SchoolController : ControllerBase
	{
		private readonly ISchoolService _schoolService;
		private readonly ILogger<SchoolController> _logger;

		public SchoolController(ISchoolService schoolService, ILogger<SchoolController> logger)
		{
			_schoolService = schoolService;
			_logger = logger;
		}

		/// <summary>
		/// Retrieves all schools
		/// </summary>
		/// <returns>A list of all schools</returns>
		/// <response code="200">Returns the list of schools</response>
		/// <response code="500">If an error occurs while retrieving schools</response>
		[HttpGet]
		[ProducesResponseType(typeof(IEnumerable<SchoolDto>), 200)]
		[ProducesResponseType(500)]
		public async Task<ActionResult<IEnumerable<SchoolDto>>> GetAllSchools()
		{
			try
			{
				var schools = await _schoolService.GetAllSchoolsAsync();
				return Ok(schools);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving schools");
				return StatusCode(500, "An error occurred while retrieving schools");
			}
		}

		/// <summary>
		/// Retrieves a specific school by ID
		/// </summary>
		/// <param name="id">The school ID</param>
		/// <returns>The school if found</returns>
		/// <response code="200">Returns the school</response>
		/// <response code="404">If the school is not found</response>
		/// <response code="500">If an error occurs while retrieving the school</response>
		[HttpGet("{id}")]
		[ProducesResponseType(typeof(SchoolDto), 200)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public async Task<ActionResult<SchoolDto>> GetSchool(Guid id)
		{
			try
			{
				var school = await _schoolService.GetSchoolByIdAsync(id);
				if (school == null)
				{
					return NotFound();
				}
				return Ok(school);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving school with ID: {SchoolId}", id);
				return StatusCode(500, "An error occurred while retrieving the school");
			}
		}

		/// <summary>
		/// Creates a new school
		/// </summary>
		/// <param name="request">The school creation request</param>
		/// <returns>The created school</returns>
		/// <response code="201">Returns the newly created school</response>
		/// <response code="400">If the request is invalid</response>
		/// <response code="500">If an error occurs while creating the school</response>
		[HttpPost]
		[ProducesResponseType(typeof(SchoolDto), 201)]
		[ProducesResponseType(400)]
		[ProducesResponseType(500)]
		public async Task<ActionResult<SchoolDto>> CreateSchool(CreateSchoolRequest request)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				var school = await _schoolService.CreateSchoolAsync(request);
				return CreatedAtAction(nameof(GetSchool), new { id = school.Id }, school);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating school");
				return StatusCode(500, "An error occurred while creating the school");
			}
		}

		/// <summary>
		/// Updates an existing school
		/// </summary>
		/// <param name="id">The school ID</param>
		/// <param name="request">The school update request</param>
		/// <returns>The updated school</returns>
		/// <response code="200">Returns the updated school</response>
		/// <response code="400">If the request is invalid or IDs don't match</response>
		/// <response code="404">If the school is not found</response>
		/// <response code="500">If an error occurs while updating the school</response>
		[HttpPut("{id}")]
		[ProducesResponseType(typeof(SchoolDto), 200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> UpdateSchool(Guid id, UpdateSchoolRequest request)
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

				var school = await _schoolService.UpdateSchoolAsync(request);
				return Ok(school);
			}
			catch (KeyNotFoundException)
			{
				return NotFound();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating school with ID: {SchoolId}", id);
				return StatusCode(500, "An error occurred while updating the school");
			}
		}

		/// <summary>
		/// Deletes a school by ID
		/// </summary>
		/// <param name="id">The school ID</param>
		/// <returns>No content if successful</returns>
		/// <response code="204">If the school is deleted successfully</response>
		/// <response code="404">If the school is not found</response>
		/// <response code="500">If an error occurs while deleting the school</response>
		[HttpDelete("{id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> DeleteSchool(Guid id)
		{
			try
			{
				var result = await _schoolService.DeleteSchoolAsync(id);
				if (!result)
				{
					return NotFound();
				}
				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting school with ID: {SchoolId}", id);
				return StatusCode(500, "An error occurred while deleting the school");
			}
		}

		/// <summary>
		/// Retrieves all schools by company ID
		/// </summary>
		/// <param name="companyId">The company ID</param>
		/// <returns>A list of schools for the specified company</returns>
		/// <response code="200">Returns the list of schools</response>
		/// <response code="500">If an error occurs while retrieving schools</response>
		[HttpGet("company/{companyId}")]
		[ProducesResponseType(typeof(IEnumerable<SchoolDto>), 200)]
		[ProducesResponseType(500)]
		public async Task<ActionResult<IEnumerable<SchoolDto>>> GetSchoolsByCompany(Guid companyId)
		{
			try
			{
				var schools = await _schoolService.GetSchoolsByCompanyIdAsync(companyId);
				return Ok(schools);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving schools for company ID: {CompanyId}", companyId);
				return StatusCode(500, "An error occurred while retrieving schools");
			}
		}
	}
}
