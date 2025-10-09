using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SchoolPortal.API.DTOs;
using SchoolPortal.API.DTOs.Company;
using SchoolPortal.API.Interfaces.Services;
using CompanyDto = SchoolPortal.API.DTOs.Company.CompanyDto;

namespace SchoolPortal.API.Controllers
{
	/// <summary>
	/// Controller for managing company operations
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	// [Authorize]
	[Produces("application/json")]
	public class CompanyController : ControllerBase
	{
		private readonly ICompanyService _companyService;
		private readonly ILogger<CompanyController> _logger;

		public CompanyController(ICompanyService companyService, ILogger<CompanyController> logger)
		{
			_companyService = companyService;
			_logger = logger;
		}

		/// <summary>
		/// Retrieves all companies
		/// </summary>
		/// <returns>A list of all companies</returns>
		/// <response code="200">Returns the list of companies</response>
		/// <response code="500">If an error occurs while retrieving companies</response>
		[HttpGet]
		[ProducesResponseType(typeof(IEnumerable<CompanyDto>), 200)]
		[ProducesResponseType(500)]
		public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAllCompanies()
		{
			try
			{
				var companies = await _companyService.GetAllCompaniesAsync();
				return Ok(companies);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving companies");
				return StatusCode(500, "An error occurred while retrieving companies");
			}
		}

		/// <summary>
		/// Retrieves a specific company by ID
		/// </summary>
		/// <param name="id">The company ID</param>
		/// <returns>The company if found</returns>
		/// <response code="200">Returns the company</response>
		/// <response code="404">If the company is not found</response>
		/// <response code="500">If an error occurs while retrieving the company</response>
		[HttpGet("{id}")]
		[ProducesResponseType(typeof(CompanyDto), 200)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public async Task<ActionResult<CompanyDto>> GetCompany(Guid id)
		{
			try
			{
				var company = await _companyService.GetCompanyByIdAsync(id);
				if (company == null)
				{
					return NotFound();
				}
				return Ok(company);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving company with ID: {CompanyId}", id);
				return StatusCode(500, "An error occurred while retrieving the company");
			}
		}

		/// <summary>
		/// Creates a new company
		/// </summary>
		/// <param name="request">The company creation request</param>
		/// <returns>The created company</returns>
		/// <response code="201">Returns the newly created company</response>
		/// <response code="400">If the request is invalid</response>
		/// <response code="500">If an error occurs while creating the company</response>
		[HttpPost]
		[ProducesResponseType(typeof(CompanyDto), 201)]
		[ProducesResponseType(400)]
		[ProducesResponseType(500)]
		public async Task<ActionResult<CompanyDto>> CreateCompany(CreateCompanyRequest request)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				var company = await _companyService.CreateCompanyAsync(request);
				return CreatedAtAction(nameof(GetCompany), new { id = company.Id }, company);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating company");
				return StatusCode(500, "An error occurred while creating the company");
			}
		}

		/// <summary>
		/// Updates an existing company
		/// </summary>
		/// <param name="id">The company ID</param>
		/// <param name="request">The company update request</param>
		/// <returns>The updated company</returns>
		/// <response code="200">Returns the updated company</response>
		/// <response code="400">If the request is invalid or IDs don't match</response>
		/// <response code="404">If the company is not found</response>
		/// <response code="500">If an error occurs while updating the company</response>
		[HttpPut("{id}")]
		[ProducesResponseType(typeof(CompanyDto), 200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> UpdateCompany(Guid id, UpdateCompanyRequest request)
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

				var company = await _companyService.UpdateCompanyAsync(request);
				return Ok(company);
			}
			catch (KeyNotFoundException)
			{
				return NotFound();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating company with ID: {CompanyId}", id);
				return StatusCode(500, "An error occurred while updating the company");
			}
		}

		/// <summary>
		/// Deletes a company by ID
		/// </summary>
		/// <param name="id">The company ID</param>
		/// <returns>No content if successful</returns>
		/// <response code="204">If the company is deleted successfully</response>
		/// <response code="404">If the company is not found</response>
		/// <response code="500">If an error occurs while deleting the company</response>
		[HttpDelete("{id}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> DeleteCompany(Guid id)
		{
			try
			{
				var result = await _companyService.DeleteCompanyAsync(id);
				if (!result)
				{
					return NotFound();
				}
				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting company with ID: {CompanyId}", id);
				return StatusCode(500, "An error occurred while deleting the company");
			}
		}

		/// <summary>
		/// Retrieves all countries with their states and cities
		/// </summary>
		/// <returns>A list of countries with states and cities</returns>
		/// <response code="200">Returns the list of countries</response>
		/// <response code="500">If an error occurs while retrieving locations</response>
		[HttpGet("locations/countries")]
		[ProducesResponseType(typeof(IEnumerable<CountryDto>), 200)]
		[ProducesResponseType(500)]
		public async Task<ActionResult<IEnumerable<CountryDto>>> GetCountriesWithStatesAndCities()
		{
			try
			{
				var countries = await _companyService.GetCountriesWithStatesAndCitiesAsync();
				return Ok(countries);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving countries with states and cities");
				return StatusCode(500, "An error occurred while retrieving locations");
			}
		}

		/// <summary>
		/// Retrieves states by country ID
		/// </summary>
		/// <param name="countryId">The country ID</param>
		/// <returns>A list of states for the specified country</returns>
		/// <response code="200">Returns the list of states</response>
		/// <response code="500">If an error occurs while retrieving states</response>
		[HttpGet("locations/states/{countryId}")]
		[ProducesResponseType(typeof(IEnumerable<StateDto>), 200)]
		[ProducesResponseType(500)]
		public async Task<ActionResult<IEnumerable<StateDto>>> GetStatesByCountry(Guid countryId)
		{
			try
			{
				var states = await _companyService.GetStatesByCountryIdAsync(countryId);
				return Ok(states);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving states for country ID: {CountryId}", countryId);
				return StatusCode(500, "An error occurred while retrieving states");
			}
		}

		/// <summary>
		/// Retrieves cities by state ID
		/// </summary>
		/// <param name="stateId">The state ID</param>
		/// <returns>A list of cities for the specified state</returns>
		/// <response code="200">Returns the list of cities</response>
		/// <response code="500">If an error occurs while retrieving cities</response>
		[HttpGet("locations/cities/{stateId}")]
		[ProducesResponseType(typeof(IEnumerable<CityDto>), 200)]
		[ProducesResponseType(500)]
		public async Task<ActionResult<IEnumerable<CityDto>>> GetCitiesByState(Guid stateId)
		{
			try
			{
				var cities = await _companyService.GetCitiesByStateIdAsync(stateId);
				return Ok(cities);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving cities for state ID: {StateId}", stateId);
				return StatusCode(500, "An error occurred while retrieving cities");
			}
		}
	}
}