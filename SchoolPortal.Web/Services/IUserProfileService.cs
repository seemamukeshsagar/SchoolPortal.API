using SchoolPortal.Web.DTOs;

namespace SchoolPortal.Web.Services
{
    public interface IUserProfileService
    {
        Task<UserProfileDto> GetUserProfileAsync(string username);
        Task<RoleDto?> GetRoleByIdAsync(Guid roleId);
        Task<DesignationDto?> GetDesignationByIdAsync(Guid designationId);
    }
}
