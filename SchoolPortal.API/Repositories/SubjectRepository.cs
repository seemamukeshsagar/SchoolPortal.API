// Repositories/SubjectRepository.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.API.Interfaces.Repositories;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly SchoolPortalNewContext _context;

        public SubjectRepository(SchoolPortalNewContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SubjectMaster>> GetAllSubjectsAsync()
        {
            return await _context.SubjectMasters
                .Include(s => s.Company)
                .Include(s => s.School)
                .Include(s => s.CreatedByNavigation)
                .Include(s => s.ModifiedByNavigation)
                .Where(s => !s.IsDeleted)
                .ToListAsync();
        }

        public async Task<SubjectMaster> GetSubjectByIdAsync(Guid id)
        {
            var subject = await _context.SubjectMasters
                .Include(s => s.Company)
                .Include(s => s.School)
                .Include(s => s.CreatedByNavigation)
                .Include(s => s.ModifiedByNavigation)
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

            if (subject == null)
                throw new KeyNotFoundException($"Subject with Id '{id}' not found.");

            return subject;
        }

        public async Task<SubjectMaster> CreateSubjectAsync(SubjectMaster subject)
        {
            subject.Id = Guid.NewGuid();
            subject.IsDeleted = false;
            subject.CreatedDate = DateTime.UtcNow;

            await _context.SubjectMasters.AddAsync(subject);
            await _context.SaveChangesAsync();

            return subject;
        }

        public async Task<SubjectMaster> UpdateSubjectAsync(SubjectMaster subject)
        {
            subject.ModifiedDate = DateTime.UtcNow;
            _context.SubjectMasters.Update(subject);
            await _context.SaveChangesAsync();
            return subject;
        }

        public async Task<bool> DeleteSubjectAsync(Guid id)
        {
            var subject = await _context.SubjectMasters
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

            if (subject == null)
                return false;

            subject.IsDeleted = true;
            subject.ModifiedDate = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SubjectMaster>> GetSubjectsBySchoolAsync(Guid schoolId)
        {
            return await _context.SubjectMasters
                .Include(s => s.Company)
                .Include(s => s.School)
                .Where(s => s.SchoolId == schoolId && !s.IsDeleted)
                .ToListAsync();
        }
    }
}