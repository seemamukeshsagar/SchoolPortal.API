using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Interfaces.Repositories
{
    public interface ICompanyRepository
    {
        Task<IEnumerable<CompanyMaster>> GetAllCompaniesAsync();
        Task<CompanyMaster> GetCompanyByIdAsync(Guid id);
        Task<CompanyMaster> CreateCompanyAsync(CompanyMaster company);
        Task<CompanyMaster> UpdateCompanyAsync(CompanyMaster company);
        Task<bool> DeleteCompanyAsync(Guid id);
        Task<IEnumerable<CountryMaster>> GetCountriesWithStatesAndCitiesAsync();
        Task<IEnumerable<StateMaster>> GetStatesByCountryIdAsync(Guid countryId);
        Task<IEnumerable<CityMaster>> GetCitiesByStateIdAsync(Guid stateId);
    }
}