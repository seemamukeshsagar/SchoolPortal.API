// SchoolPortal.API\Interfaces\Repositories\IRoleRepository.cs
using SchoolPortal.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolPortal.API.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<RoleMaster> GetByIdAsync(Guid id);
        Task<IEnumerable<RoleMaster>> GetAllAsync();
        Task<IEnumerable<RoleMaster>> GetByCompanyIdAsync(Guid companyId);
        Task<IEnumerable<RoleMaster>> GetBySchoolIdAsync(Guid schoolId);
        Task<RoleMaster> AddAsync(RoleMaster role);
        Task UpdateAsync(RoleMaster role);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> NameExistsAsync(string name, Guid companyId, Guid? excludeId = null);
    }
}