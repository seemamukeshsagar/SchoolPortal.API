using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolPortal.API.DTOs;
using SchoolPortal.API.Interfaces;

namespace SchoolPortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationsController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet("countries")]
        public async Task<ActionResult<IEnumerable<CountryDto>>> GetCountries()
        {
            var countries = await _locationService.GetCountriesAsync();
            return Ok(countries);
        }

        [HttpGet("states/{countryId}")]
        public async Task<ActionResult<IEnumerable<StateDto>>> GetStatesByCountry(Guid countryId)
        {
            var states = await _locationService.GetStatesByCountryAsync(countryId);
            return Ok(states);
        }

        [HttpGet("cities/{stateId}")]
        public async Task<ActionResult<IEnumerable<CityDto>>> GetCitiesByState(Guid stateId)
        {
            var cities = await _locationService.GetCitiesByStateAsync(stateId);
            return Ok(cities);
        }

        [HttpGet("jurisdiction-areas/{stateId}")]
        public async Task<ActionResult<IEnumerable<CityDto>>> GetJurisdictionAreas(Guid stateId)
        {
            var areas = await _locationService.GetJurisdictionAreasAsync(stateId);
            return Ok(areas);
        }
    }
}
