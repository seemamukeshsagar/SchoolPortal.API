using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.API.Interfaces.Repositories;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Repositories
{
	public class SchoolRepository : ISchoolRepository
	{
		private readonly SchoolPortalNewContext _context;

		public SchoolRepository(SchoolPortalNewContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<SchoolMaster>> GetAllSchoolsAsync()
		{
			return await _context.SchoolMasters
				.Include(s => s.Company)
				.Include(s => s.Country)
				.Include(s => s.State)
				.Include(s => s.City)
				.Include(s => s.JudistrictionCountry)
				.Include(s => s.JudistrictionState)
				.Include(s => s.JudistrictionCity)
				.Include(s => s.BankCountry)
				.Include(s => s.BankState)
				.Include(s => s.BankCity)
				.Where(s => !s.IsDeleted)
				.ToListAsync();
		}

		public async Task<SchoolMaster> GetSchoolByIdAsync(Guid id)
		{
			var school = await _context.SchoolMasters
				.Include(s => s.Company)
				.Include(s => s.Country)
				.Include(s => s.State)
				.Include(s => s.City)
				.Include(s => s.JudistrictionCountry)
				.Include(s => s.JudistrictionState)
				.Include(s => s.JudistrictionCity)
				.Include(s => s.BankCountry)
				.Include(s => s.BankState)
				.Include(s => s.BankCity)
				.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

			if (school == null)
				throw new InvalidOperationException($"School with Id '{id}' not found.");

			return school;
		}

		public async Task<SchoolMaster> CreateSchoolAsync(SchoolMaster school)
		{
			school.Id = Guid.NewGuid();
			school.IsDeleted = false;
			school.CreatedDate = DateTime.UtcNow;

			await _context.SchoolMasters.AddAsync(school);
			await _context.SaveChangesAsync();

			return school;
		}

		public async Task<SchoolMaster> UpdateSchoolAsync(SchoolMaster school)
		{
			school.ModifiedDate = DateTime.UtcNow;
			_context.SchoolMasters.Update(school);
			await _context.SaveChangesAsync();
			return school;
		}

		public async Task<bool> DeleteSchoolAsync(Guid id)
		{
			var school = await _context.SchoolMasters
				.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

			if (school == null)
				return false;

			school.IsDeleted = true;
			school.ModifiedDate = DateTime.UtcNow;
			
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<IEnumerable<SchoolMaster>> GetSchoolsByCompanyIdAsync(Guid companyId)
		{
			return await _context.SchoolMasters
				.Include(s => s.Company)
				.Include(s => s.Country)
				.Include(s => s.State)
				.Include(s => s.City)
				.Where(s => s.CompanyId == companyId && !s.IsDeleted)
				.ToListAsync();
		}
	}
}
