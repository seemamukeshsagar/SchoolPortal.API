// In SchoolPortal.API/Services/LocationService.cs
using Microsoft.EntityFrameworkCore;
using SchoolPortal.API.DTOs;
using SchoolPortal.API.Interfaces;
using SchoolPortal.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.API.Services
{
    public class LocationService : ILocationService
    {
        private readonly SchoolPortalNewContext _context;

        public LocationService(SchoolPortalNewContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<CountryDto>> GetCountriesAsync()
        {
            if (_context?.CountryMasters == null)
                return new List<CountryDto>();

            return await _context.CountryMasters
                .Select(c => new CountryDto
                {
                    Id = c.Id,
                    CountryName = c.CountryName ?? string.Empty,
                    IsActive = c.IsActive
                    // Add other required properties if needed
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<StateDto>> GetStatesByCountryAsync(Guid countryId) 
        {
            if (countryId == Guid.Empty || _context?.StateMasters == null)
                return new List<StateDto>();

            return await _context.StateMasters
                .Where(s => s.CountryId == countryId)
                .Select(s => new StateDto
                {
                    Id = s.Id,
                    StateName = s.StateName ?? string.Empty,
                    CountryId = s.CountryId,
                    IsActive = s.IsActive
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<CityDto>> GetCitiesByStateAsync(Guid stateId) 
        {
            if (stateId == Guid.Empty || _context?.CityMasters == null)
                return new List<CityDto>();

            return await _context.CityMasters
                .Where(c => c.CityStateId == stateId)
                .Select(c => new CityDto
                {
                    Id = c.Id,
                    CityName = c.CityName ?? string.Empty,
                    StateId = c.CityStateId
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<CityDto>> GetJurisdictionAreasAsync(Guid stateId)
        {
            if (stateId == Guid.Empty || _context?.CityMasters == null)
                return new List<CityDto>();

            return await _context.CityMasters
                .Where(c => c.CityStateId == stateId)
                .OrderBy(c => c.CityName)
                .Select(c => new CityDto
                {
                    Id = c.Id,
                    CityName = c.CityName ?? string.Empty,
                    StateId = c.CityStateId
                })
                .ToListAsync();
        }
    }
}