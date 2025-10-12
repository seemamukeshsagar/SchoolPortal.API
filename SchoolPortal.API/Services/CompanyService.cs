using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SchoolPortal.API.DTOs;
using SchoolPortal.API.DTOs.Company;
using SchoolPortal.API.Interfaces.Repositories;
using SchoolPortal.API.Interfaces.Services;
using SchoolPortal.API.Models;
using CompanyDto = SchoolPortal.API.DTOs.Company.CompanyDto;

namespace SchoolPortal.API.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public CompanyService(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync()
        {
            var companies = await _companyRepository.GetAllCompaniesAsync();
            return _mapper.Map<IEnumerable<CompanyDto>>(companies);
        }

        public async Task<CompanyDto> GetCompanyByIdAsync(Guid id)
        {
            var company = await _companyRepository.GetCompanyByIdAsync(id);
            return _mapper.Map<CompanyDto>(company);
        }

        public async Task<CompanyDto> CreateCompanyAsync(CreateCompanyRequest request)
        {
            var company = _mapper.Map<CompanyMaster>(request);
            var createdCompany = await _companyRepository.CreateCompanyAsync(company);
            return _mapper.Map<CompanyDto>(createdCompany);
        }

        public async Task<CompanyDto> UpdateCompanyAsync(UpdateCompanyRequest request)
        {
            var existingCompany = await _companyRepository.GetCompanyByIdAsync(request.Id);
            if (existingCompany == null)
                throw new KeyNotFoundException("Company not found");

            _mapper.Map(request, existingCompany);
            var updatedCompany = await _companyRepository.UpdateCompanyAsync(existingCompany);
            return _mapper.Map<CompanyDto>(updatedCompany);
        }

        public async Task<bool> DeleteCompanyAsync(Guid id)
        {
            return await _companyRepository.DeleteCompanyAsync(id);
        }

        public async Task<IEnumerable<CountryDto>> GetCountriesWithStatesAndCitiesAsync()
        {
            var countries = await _companyRepository.GetCountriesWithStatesAndCitiesAsync();
            return _mapper.Map<IEnumerable<CountryDto>>(countries);
        }

        public async Task<IEnumerable<StateDto>> GetStatesByCountryIdAsync(Guid countryId)
        {
            var states = await _companyRepository.GetStatesByCountryIdAsync(countryId);
            return _mapper.Map<IEnumerable<StateDto>>(states);
        }

        public async Task<IEnumerable<CityDto>> GetCitiesByStateIdAsync(Guid stateId)
        {
            var cities = await _companyRepository.GetCitiesByStateIdAsync(stateId);
            return _mapper.Map<IEnumerable<CityDto>>(cities);
        }
    }
}