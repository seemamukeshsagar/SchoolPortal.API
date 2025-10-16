using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SchoolPortal.API.DTOs.School;
using SchoolPortal.API.Interfaces.Services;

namespace SchoolPortal.API.Controllers
{
	/// <summary>
	/// Controller for managing school contact operations
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	[Produces("application/json")]
	[Authorize]
	public class SchoolContactController : ControllerBase
	{
		private readonly ISchoolContactService _schoolContactService;
		private readonly ILogger<SchoolContactController> _logger;

		public SchoolContactController(
			ISchoolContactService schoolContactService,
			ILogger<SchoolContactController> logger)
		{
			_schoolContactService = schoolContactService ?? throw new ArgumentNullException(nameof(schoolContactService));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <summary>
		/// Gets all contacts for a school
		/// </summary>
		/// <param name="schoolId">The school ID</param>
		/// <returns>List of school contacts</returns>
		/// <response code="200">Returns the list of contacts</response>
		/// <response code="500">If an error occurs while retrieving contacts</response>
		[HttpGet("school/{schoolId}")]
		[ProducesResponseType(typeof(IEnumerable<SchoolContactDto>), 200)]
		[ProducesResponseType(500)]
		public async Task<ActionResult<IEnumerable<SchoolContactDto>>> GetBySchoolId(Guid schoolId)
		{
			try
			{
				var contacts = await _schoolContactService.GetContactsBySchoolIdAsync(schoolId);
				return Ok(contacts);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving contacts for school {SchoolId}", schoolId);
				return StatusCode(500, "An error occurred while retrieving contacts");
			}
		}

		/// <summary>
		/// Gets a specific contact by ID
		/// </summary>
		/// <param name="id">The contact ID</param>
		/// <returns>The contact if found</returns>
		/// <response code="200">Returns the contact</response>
		/// <response code="404">If the contact is not found</response>
		/// <response code="500">If an error occurs while retrieving the contact</response>
		[HttpGet("{id}")]
		[ProducesResponseType(typeof(SchoolContactDto), 200)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public async Task<ActionResult<SchoolContactDto>> GetById(Guid id)
		{
			try
			{
				var contact = await _schoolContactService.GetContactByIdAsync(id);
				if (contact == null)
				{
					return NotFound();
				}
				return Ok(contact);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving contact {ContactId}", id);
				return StatusCode(500, "An error occurred while retrieving the contact");
			}
		}

		/// <summary>
		/// Creates a new school contact
		/// </summary>
		/// <param name="request">The contact creation request</param>
		/// <returns>The created contact</returns>
		/// <response code="201">Returns the newly created contact</response>
		/// <response code="400">If the request is invalid</response>
		/// <response code="500">If an error occurs while creating the contact</response>
		[HttpPost]
		[ProducesResponseType(typeof(SchoolContactDto), 201)]
		[ProducesResponseType(400)]
		[ProducesResponseType(500)]
		public async Task<ActionResult<SchoolContactDto>> Create([FromBody] SchoolContactRequest request)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				var contact = await _schoolContactService.CreateContactAsync(request);
				return CreatedAtAction(nameof(GetById), new { id = contact.Id }, contact);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating contact for school {SchoolId}", request.SchoolId);
				return StatusCode(500, "An error occurred while creating the contact");
			}
		}

		/// <summary>
		/// Updates an existing school contact
		/// </summary>
		/// <param name="id">The contact ID</param>
		/// <param name="request">The contact update request</param>
		/// <returns>The updated contact</returns>
		/// <response code="200">Returns the updated contact</response>
		/// <response code="400">If the request is invalid</response>
		/// <response code="404">If the contact is not found</response>
		/// <response code="500">If an error occurs while updating the contact</response>
		[HttpPut("{id}")]
		[ProducesResponseType(typeof(SchoolContactDto), 200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public async Task<ActionResult<SchoolContactDto>> Update(Guid id, [FromBody] SchoolContactRequest request)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				var contact = await _schoolContactService.UpdateContactAsync(id, request);
				if (contact == null)
				{
					return NotFound();
				}

				return Ok(contact);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating contact {ContactId}", id);
				return StatusCode(500, "An error occurred while updating the contact");
			}
		}

		/// <summary>
		/// Deletes a school contact
		/// </summary>
		/// <param name="id">The contact ID</param>
		/// <returns>No content if successful</returns>
		/// <response code="204">If the contact was deleted successfully</response>
		/// <response code="404">If the contact is not found</response>
		/// <response code="500">If an error occurs while deleting the contact</response>
		[HttpDelete("{id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> Delete(Guid id)
		{
			try
			{
				var result = await _schoolContactService.DeleteContactAsync(id);
				if (!result)
				{
					return NotFound();
				}

				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting contact {ContactId}", id);
				return StatusCode(500, "An error occurred while deleting the contact");
			}
		}
	}
}
