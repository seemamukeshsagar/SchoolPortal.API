using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.API.Data;
using SchoolPortal.API.Interfaces.Repositories;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Data.Repositories
{
    public class SchoolContactRepository : Repository<SchoolContactMaster>, ISchoolContactRepository
    {
        public SchoolContactRepository(SchoolPortalNewContext context) : base(context)
        {
        }

        public async Task<IEnumerable<SchoolContactMaster>> GetBySchoolIdWithDetailsAsync(Guid schoolId)
        {
            return await _context.SchoolContactMasters
                .Include(c => c.City)
                .Include(c => c.State)
                .Include(c => c.Country)
                .Where(c => c.SchoolId == schoolId && !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<SchoolContactMaster> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.SchoolContactMasters
                .Include(c => c.City)
                .Include(c => c.State)
                .Include(c => c.Country)
                .Include(c => c.School)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }
    }
}
