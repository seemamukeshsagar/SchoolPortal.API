using Microsoft.EntityFrameworkCore;
using SchoolPortal.API.Data;
using SchoolPortal.API.DTOs.Privilege;
using SchoolPortal.API.Interfaces.Repositories;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Repositories
{
    public class PrivilegeRepository : IPrivilegeRepository
    {
        private readonly SchoolPortalNewContext _context;

        public PrivilegeRepository(SchoolPortalNewContext context)
        {
            _context = context;
        }

        public async Task<Privilege> AddAsync(Privilege privilege)
        {
            await _context.Privileges.AddAsync(privilege);
            await _context.SaveChangesAsync();
            return privilege;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var privilege = await _context.Privileges.FindAsync(id);
            if (privilege == null) return false;

            privilege.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Privileges.AnyAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<IEnumerable<PrivilegeListDto>> GetAllAsync()
        {
            return await _context.Privileges
                .Where(p => !p.IsDeleted)
                .Select(p => new PrivilegeListDto
                {
                    Id = p.Id,
                    PrivilegeName = p.PrivilegeName!,
                    ParentPrivilegeName = _context.Privileges
                        .Where(pp => pp.PrivilegeParentId == p.PrivilegeParentId)
                        .Select(pp => pp.PrivilegeName)
                        .FirstOrDefault(),
                    IsActive = p.IsActive
                })
                .ToListAsync();
        }

        public async Task<PrivilegeDto?> GetByIdAsync(Guid id)
        {
            return await _context.Privileges
                .Where(p => p.Id == id && !p.IsDeleted)
                .Select(p => new PrivilegeDto
                {
                    Id = p.Id,
                    PrivilegeName = p.PrivilegeName!,
                    PrivilegeParentId = p.PrivilegeParentId,
                    ParentPrivilegeName = _context.Privileges
                        .Where(pp => pp.PrivilegeParentId == p.PrivilegeParentId)
                        .Select(pp => pp.PrivilegeName)
                        .FirstOrDefault(),
                    IsActive = p.IsActive,
                    IsDeleted = p.IsDeleted,
                    CreatedDate = p.CreatedDate,
                    ModifiedDate = p.ModifiedDate,
                    Status = p.Status,
                    StatusMessage = p.StatusMessage
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Privilege?> UpdateAsync(Privilege privilege)
        {
            var existingPrivilege = await _context.Privileges.FindAsync(privilege.Id);
            if (existingPrivilege == null) return null;

            _context.Entry(existingPrivilege).CurrentValues.SetValues(privilege);
            existingPrivilege.ModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existingPrivilege;
        }
    }
}