using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.API.Interfaces.Repositories;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Repositories
{
	public class CompanyRepository : ICompanyRepository
	{
		private readonly SchoolPortalNewContext _context;

		public CompanyRepository(SchoolPortalNewContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<CompanyMaster>> GetAllCompaniesAsync()
		{
			return await _context.CompanyMasters
				.Include(c => c.Country)
				.Include(c => c.State)
				.Include(c => c.City)
				.Where(c => !c.IsDeleted)
				.ToListAsync();
		}

		public async Task<CompanyMaster> GetCompanyByIdAsync(Guid id)
		{
			var company = await _context.CompanyMasters
				.Include(c => c.Country)
				.Include(c => c.State)
				.Include(c => c.City)
				.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

			if (company == null)
				throw new InvalidOperationException($"Company with Id '{id}' not found.");

			return company;
		}

		public async Task<CompanyMaster> CreateCompanyAsync(CompanyMaster company)
		{
			company.Id = Guid.NewGuid();
			company.IsDeleted = false;
			company.CreatedDate = DateTime.UtcNow;

			await _context.CompanyMasters.AddAsync(company);
			await _context.SaveChangesAsync();

			return company;
		}

		public async Task<CompanyMaster> UpdateCompanyAsync(CompanyMaster company)
		{
			company.ModifiedDate = DateTime.UtcNow;
			_context.CompanyMasters.Update(company);
			await _context.SaveChangesAsync();
			return company;
		}

		public async Task<bool> DeleteCompanyAsync(Guid id)
		{
			var company = await _context.CompanyMasters
				.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

			if (company == null)
				return false;

			company.IsDeleted = true;
			company.ModifiedDate = DateTime.UtcNow;
			
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<IEnumerable<CountryMaster>> GetCountriesWithStatesAndCitiesAsync()
		{
			return await _context.CountryMasters
				.Include(c => c.StateMasters)
					.ThenInclude(s => s.CityMasters)
				.Where(c => c.IsActive && !c.IsDeleted)
				.ToListAsync();
		}

		public async Task<IEnumerable<StateMaster>> GetStatesByCountryIdAsync(Guid countryId)
		{
			return await _context.StateMasters
				.Where(s => s.CountryId == countryId && s.IsActive && !s.IsDeleted)
				.ToListAsync();
		}

		public async Task<IEnumerable<CityMaster>> GetCitiesByStateIdAsync(Guid stateId)
		{
			return await _context.CityMasters
				.Where(c => c.CityStateId == stateId && c.IsActive && !c.IsDeleted)
				.ToListAsync();
		}
	}
}