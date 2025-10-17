// SchoolPortal.API\Interfaces\Services\IRoleService.cs
using SchoolPortal.API.DTOs.Role;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolPortal.API.Interfaces.Services
{
    public interface IRoleService
    {
        Task<RoleDto> GetByIdAsync(Guid id);
        Task<IEnumerable<RoleListDto>> GetAllAsync();
        Task<IEnumerable<RoleListDto>> GetByCompanyIdAsync(Guid companyId);
        Task<IEnumerable<RoleListDto>> GetBySchoolIdAsync(Guid schoolId);
        Task<RoleDto> CreateAsync(CreateRoleRequest request, Guid currentUserId);
        Task<RoleDto> UpdateAsync(UpdateRoleRequest request, Guid currentUserId);
        Task DeleteAsync(Guid id, Guid currentUserId);
    }
}