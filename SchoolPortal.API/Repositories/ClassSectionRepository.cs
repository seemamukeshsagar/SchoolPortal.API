using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.API.Models;  // This is where SchoolPortalNewContext is
using SchoolPortal.API.Interfaces.Repositories;

namespace SchoolPortal.API.Data.Repositories
{
    public class ClassSectionRepository : IClassSectionRepository
    {
        private readonly SchoolPortalNewContext _context;

        public ClassSectionRepository(SchoolPortalNewContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<ClassSectionDetail>> GetAllClassSectionsAsync()
        {
            return await _context.ClassSectionDetails
                .Include(cs => cs.ClassMaster)
                .Include(cs => cs.SectionMaster)
                .Include(cs => cs.ClassTeacher)
                .ToListAsync();
        }

        public async Task<ClassSectionDetail> GetClassSectionByIdAsync(Guid id)
        {
            return await _context.ClassSectionDetails
                .Include(cs => cs.ClassMaster)
                .Include(cs => cs.SectionMaster)
                .Include(cs => cs.ClassTeacher)
                .FirstOrDefaultAsync(cs => cs.Id == id);
        }

        public async Task<ClassSectionDetail> CreateClassSectionAsync(ClassSectionDetail classSection)
        {
            _context.ClassSectionDetails.Add(classSection);
            await _context.SaveChangesAsync();
            return classSection;
        }

        public async Task<ClassSectionDetail> UpdateClassSectionAsync(ClassSectionDetail classSection)
        {
            _context.Entry(classSection).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return classSection;
        }

        public async Task<bool> DeleteClassSectionAsync(Guid id)
        {
            var classSection = await _context.ClassSectionDetails.FindAsync(id);
            if (classSection == null)
                return false;

            _context.ClassSectionDetails.Remove(classSection);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}