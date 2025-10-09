using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SchoolPortal.API.DTOs.Company;
using SchoolPortal.API.Interfaces.Services;

namespace SchoolPortal.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class CompanyController : ControllerBase
	{
		private readonly ICompanyService _companyService;
		private readonly ILogger<CompanyController> _logger;

		public CompanyController(ICompanyService companyService, ILogger<CompanyController> logger)
		{
			_companyService = companyService;
			_logger = logger;
		}

		[HttpGet]
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

		[HttpGet("{id}")]
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
				_logger.LogError(ex, $"Error retrieving company with ID: {id}");
				return StatusCode(500, "An error occurred while retrieving the company");
			}
		}

		[HttpPost]
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

		[HttpPut("{id}")]
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
				_logger.LogError(ex, $"Error updating company with ID: {id}");
				return StatusCode(500, "An error occurred while updating the company");
			}
		}

		[HttpDelete("{id}")]
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
				_logger.LogError(ex, $"Error deleting company with ID: {id}");
				return StatusCode(500, "An error occurred while deleting the company");
			}
		}

		[HttpGet("locations/countries")]
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

		[HttpGet("locations/states/{countryId}")]
		public async Task<ActionResult<IEnumerable<StateDto>>> GetStatesByCountry(Guid countryId)
		{
			try
			{
				var states = await _companyService.GetStatesByCountryIdAsync(countryId);
				return Ok(states);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error retrieving states for country ID: {countryId}");
				return StatusCode(500, "An error occurred while retrieving states");
			}
		}

		[HttpGet("locations/cities/{stateId}")]
		public async Task<ActionResult<IEnumerable<CityDto>>> GetCitiesByState(Guid stateId)
		{
			try
			{
				var cities = await _companyService.GetCitiesByStateIdAsync(stateId);
				return Ok(cities);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error retrieving cities for state ID: {stateId}");
				return StatusCode(500, "An error occurred while retrieving cities");
			}
		}
	}
}