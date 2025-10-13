using SchoolPortal.API.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolPortal.API.Interfaces
{
    public interface ILocationService
{
    Task<IEnumerable<CountryDto>> GetCountriesAsync();
    Task<IEnumerable<StateDto>> GetStatesByCountryAsync(Guid countryId);
    Task<IEnumerable<CityDto>> GetCitiesByStateAsync(Guid stateId);
    Task<IEnumerable<CityDto>> GetJurisdictionAreasAsync(Guid stateId);  // Added stateId parameter
}
}