using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolPortal.API.DTOs.Company;

namespace SchoolPortal.API.Interfaces.Services
{
    public interface ICompanyService
    {
        Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync();
        Task<CompanyDto> GetCompanyByIdAsync(Guid id);
        Task<CompanyDto> CreateCompanyAsync(CreateCompanyRequest request);
        Task<CompanyDto> UpdateCompanyAsync(UpdateCompanyRequest request);
        Task<bool> DeleteCompanyAsync(Guid id);
        Task<IEnumerable<CountryDto>> GetCountriesWithStatesAndCitiesAsync();
        Task<IEnumerable<StateDto>> GetStatesByCountryIdAsync(Guid countryId);
        Task<IEnumerable<CityDto>> GetCitiesByStateIdAsync(Guid stateId);
    }
}