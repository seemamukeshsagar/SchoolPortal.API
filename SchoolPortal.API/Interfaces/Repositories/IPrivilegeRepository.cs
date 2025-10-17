using SchoolPortal.API.DTOs.Privilege;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Interfaces.Repositories
{
    public interface IPrivilegeRepository
    {
        Task<IEnumerable<PrivilegeListDto>> GetAllAsync();
        Task<PrivilegeDto?> GetByIdAsync(Guid id);
        Task<Privilege> AddAsync(Privilege privilege);
        Task<Privilege?> UpdateAsync(Privilege privilege);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}