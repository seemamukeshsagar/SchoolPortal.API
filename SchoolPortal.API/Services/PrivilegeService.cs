using AutoMapper;
using SchoolPortal.API.DTOs.Privilege;
using SchoolPortal.API.Interfaces.Repositories;
using SchoolPortal.API.Interfaces.Services;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Services
{
    public class PrivilegeService : IPrivilegeService
    {
        private readonly IPrivilegeRepository _privilegeRepository;
        private readonly IMapper _mapper;

        public PrivilegeService(IPrivilegeRepository privilegeRepository, IMapper mapper)
        {
            _privilegeRepository = privilegeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PrivilegeListDto>> GetAllPrivilegesAsync()
        {
            return await _privilegeRepository.GetAllAsync();
        }

        public async Task<PrivilegeDto?> GetPrivilegeByIdAsync(Guid id)
        {
            return await _privilegeRepository.GetByIdAsync(id);
        }

        public async Task<PrivilegeDto> CreatePrivilegeAsync(CreatePrivilegeRequest request)
        {
            var privilege = new Privilege
            {
                PrivilegeName = request.PrivilegeName,
                PrivilegeParentId = request.PrivilegeParentId,
                IsActive = request.IsActive,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            };

            var createdPrivilege = await _privilegeRepository.AddAsync(privilege);
            return _mapper.Map<PrivilegeDto>(createdPrivilege);
        }

        public async Task<PrivilegeDto?> UpdatePrivilegeAsync(Guid id, UpdatePrivilegeRequest request)
        {
            var existingPrivilege = await _privilegeRepository.GetByIdAsync(id);
            if (existingPrivilege == null) return null;

            var privilege = new Privilege
            {
                Id = id,
                PrivilegeName = request.PrivilegeName,
                PrivilegeParentId = request.PrivilegeParentId,
                IsActive = request.IsActive,
                ModifiedDate = DateTime.UtcNow
            };

            var updatedPrivilege = await _privilegeRepository.UpdateAsync(privilege);
            return updatedPrivilege != null ? _mapper.Map<PrivilegeDto>(updatedPrivilege) : null;
        }

        public async Task<bool> DeletePrivilegeAsync(Guid id)
        {
            return await _privilegeRepository.DeleteAsync(id);
        }
    }
}