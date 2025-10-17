using SchoolPortal.API.DTOs.Privilege;

namespace SchoolPortal.API.Interfaces.Services
{
    public interface IPrivilegeService
    {
        Task<IEnumerable<PrivilegeListDto>> GetAllPrivilegesAsync();
        Task<PrivilegeDto?> GetPrivilegeByIdAsync(Guid id);
        Task<PrivilegeDto> CreatePrivilegeAsync(CreatePrivilegeRequest request);
        Task<PrivilegeDto?> UpdatePrivilegeAsync(Guid id, UpdatePrivilegeRequest request);
        Task<bool> DeletePrivilegeAsync(Guid id);
    }
}