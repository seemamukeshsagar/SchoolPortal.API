using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.API.Data;
using SchoolPortal.API.Interfaces.Repositories;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Repositories
{
    public class SectionRepository : ISectionRepository
    {
        private readonly SchoolPortalNewContext _context;

        public SectionRepository(SchoolPortalNewContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SectionMaster>> GetAllSectionsAsync()
        {
            try
            {
                return await _context.SectionMasters
                    .Include(s => s.Company)
                    .Include(s => s.School)
                    .Where(s => !s.IsDeleted)
                    .OrderBy(s => s.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the database error and return empty list for now
                Console.WriteLine($"Database error in GetAllSectionsAsync: {ex.Message}");
                return new List<SectionMaster>();
            }
        }

        public async Task<SectionMaster?> GetSectionByIdAsync(Guid id)
        {
            try
            {
                return await _context.SectionMasters
                    .Include(s => s.Company)
                    .Include(s => s.School)
                    .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error in GetSectionByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<SectionMaster> CreateSectionAsync(SectionMaster section)
        {
            try
            {
                section.CreatedDate = DateTime.UtcNow;
                section.IsDeleted = false;
                
                await _context.SectionMasters.AddAsync(section);
                await _context.SaveChangesAsync();
                
                // Reload to get related data
                return await GetSectionByIdAsync(section.Id) ?? section;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error in CreateSectionAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<SectionMaster> UpdateSectionAsync(SectionMaster section)
        {
            try
            {
                section.ModifiedDate = DateTime.UtcNow;
                _context.SectionMasters.Update(section);
                await _context.SaveChangesAsync();
                return section;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error in UpdateSectionAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteSectionAsync(Guid id)
        {
            try
            {
                var section = await _context.SectionMasters
                    .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

                if (section == null)
                {
                    return false;
                }

                section.IsDeleted = true;
                section.ModifiedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error in DeleteSectionAsync: {ex.Message}");
                throw;
            }
        }
    }
}