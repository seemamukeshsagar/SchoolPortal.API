using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.API.Interfaces.Repositories;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Repositories
{
    public class RoleRepository : SchoolPortal.API.Interfaces.Repositories.IRoleRepository
    {
        private readonly SchoolPortalNewContext _context;

        public RoleRepository() { }

        public RoleRepository(SchoolPortalNewContext context)
        {
            _context = context;
        }

        public async Task<RoleMaster> GetByIdAsync(Guid id)
        {
            return await _context.RoleMasters
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        }

        public async Task<IEnumerable<RoleMaster>> GetAllAsync()
        {
            return await _context.RoleMasters
                .AsNoTracking()
                .Where(r => !r.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<RoleMaster>> GetByCompanyIdAsync(Guid companyId)
        {
            return await _context.RoleMasters
                .AsNoTracking()
                .Where(r => r.CompanyId == companyId && !r.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<RoleMaster>> GetBySchoolIdAsync(Guid schoolId)
        {
            return await _context.RoleMasters
                .AsNoTracking()
                .Where(r => r.SchoolId == schoolId && !r.IsDeleted)
                .ToListAsync();
        }

        public async Task<RoleMaster> AddAsync(RoleMaster role)
        {
            role.Id = Guid.NewGuid();
            role.CreatedDate = DateTime.UtcNow;
            role.IsDeleted = false;

            await _context.RoleMasters.AddAsync(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task UpdateAsync(RoleMaster role)
        {
            role.ModifiedDate = DateTime.UtcNow;
            _context.RoleMasters.Update(role);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var role = await _context.RoleMasters.FindAsync(id);
            if (role != null)
            {
                role.IsDeleted = true;
                role.ModifiedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.RoleMasters
                .AnyAsync(r => r.Id == id && !r.IsDeleted);
        }

        public async Task<bool> NameExistsAsync(string name, Guid companyId, Guid? excludeId = null)
        {
            return await _context.RoleMasters
                .AnyAsync(r => r.Name.ToLower() == name.ToLower() && 
                             r.CompanyId == companyId && 
                             !r.IsDeleted &&
                             (excludeId == null || r.Id != excludeId));
        }
    }
}