// SchoolPortal.API/Data/Repositories/ClassSubjectDetailRepository.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.API.Data;
using SchoolPortal.API.Interfaces.Repositories;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Data.Repositories
{
    public class ClassSubjectDetailRepository : IClassSubjectDetailRepository
    {
        private readonly SchoolPortalNewContext _context;

        public ClassSubjectDetailRepository(SchoolPortalNewContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClassSubjectDetail>> GetAllAsync()
        {
            return await _context.ClassSubjectDetails
                .Include(csd => csd.ClassId)
                .Include(csd => csd.Subject)
                .ToListAsync();
        }

        public async Task<ClassSubjectDetail> GetByIdAsync(Guid id)
        {
            return await _context.ClassSubjectDetails
                .Include(csd => csd.ClassMaster)
                .Include(csd => csd.Subject)
                .FirstOrDefaultAsync(csd => csd.Id == id);
        }

        public async Task AddAsync(ClassSubjectDetail classSubjectDetail)
        {
            await _context.ClassSubjectDetails.AddAsync(classSubjectDetail);
        }

        public void Update(ClassSubjectDetail classSubjectDetail)
        {
            _context.Entry(classSubjectDetail).State = EntityState.Modified;
        }

        public void Delete(ClassSubjectDetail classSubjectDetail)
        {
            _context.ClassSubjectDetails.Remove(classSubjectDetail);
        }
    }
}