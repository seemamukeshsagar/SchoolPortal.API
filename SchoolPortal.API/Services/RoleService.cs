// SchoolPortal.API\Services\RoleService.cs
using AutoMapper;
using Microsoft.Extensions.Logging;
using SchoolPortal.API.DTOs.Role;
using SchoolPortal.API.Interfaces.Repositories;
using SchoolPortal.API.Interfaces.Services;
using SchoolPortal.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolPortal.API.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RoleService> _logger;

        public RoleService(
            IRoleRepository roleRepository,
            IMapper mapper,
            ILogger<RoleService> logger)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<RoleDto> GetByIdAsync(Guid id)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(id);
                if (role == null)
                {
                    _logger.LogWarning("Role with ID {RoleId} not found", id);
                    return null;
                }

                // Map to DTO and include related data
                var roleDto = _mapper.Map<RoleDto>(role);
                
                // Note: You'll need to fetch and map CompanyName and SchoolName
                // This would typically involve additional repository calls
                // roleDto.CompanyName = ...
                // roleDto.SchoolName = ...

                return roleDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role by ID: {RoleId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<RoleListDto>> GetAllAsync()
        {
            try
            {
                var roles = await _roleRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<RoleListDto>>(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all roles");
                throw;
            }
        }

        public async Task<IEnumerable<RoleListDto>> GetByCompanyIdAsync(Guid companyId)
        {
            try
            {
                var roles = await _roleRepository.GetByCompanyIdAsync(companyId);
                return _mapper.Map<IEnumerable<RoleListDto>>(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting roles by company ID: {CompanyId}", companyId);
                throw;
            }
        }

        public async Task<IEnumerable<RoleListDto>> GetBySchoolIdAsync(Guid schoolId)
        {
            try
            {
                var roles = await _roleRepository.GetBySchoolIdAsync(schoolId);
                return _mapper.Map<IEnumerable<RoleListDto>>(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting roles by school ID: {SchoolId}", schoolId);
                throw;
            }
        }

        public async Task<RoleDto> CreateAsync(CreateRoleRequest request, Guid currentUserId)
        {
            try
            {
                // Check if role name already exists
                if (await _roleRepository.NameExistsAsync(request.Name, request.CompanyId))
                {
                    throw new InvalidOperationException($"A role with the name '{request.Name}' already exists in this company.");
                }

                var role = _mapper.Map<RoleMaster>(request);
                role.CreatedBy = currentUserId;
                role.IsActive = true;

                var createdRole = await _roleRepository.AddAsync(role);
                return _mapper.Map<RoleDto>(createdRole);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role");
                throw;
            }
        }

        public async Task<RoleDto> UpdateAsync(UpdateRoleRequest request, Guid currentUserId)
        {
            try
            {
                var existingRole = await _roleRepository.GetByIdAsync(request.Id);
                if (existingRole == null)
                {
                    throw new KeyNotFoundException($"Role with ID {request.Id} not found");
                }

                // Check if another role with the same name exists (excluding current role)
                if (await _roleRepository.NameExistsAsync(request.Name, request.CompanyId, request.Id))
                {
                    throw new InvalidOperationException($"A role with the name '{request.Name}' already exists in this company.");
                }

                // Update properties
                existingRole.Name = request.Name;
                existingRole.Description = request.Description;
                existingRole.CompanyId = request.CompanyId;
                existingRole.SchoolId = request.SchoolId;
                existingRole.IsActive = request.IsActive;
                existingRole.ModifiedBy = currentUserId;

                await _roleRepository.UpdateAsync(existingRole);
                return _mapper.Map<RoleDto>(existingRole);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role with ID: {RoleId}", request.Id);
                throw;
            }
        }

        public async Task DeleteAsync(Guid id, Guid currentUserId)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(id);
                if (role == null)
                {
                    throw new KeyNotFoundException($"Role with ID {id} not found");
                }

                // Soft delete
                role.IsDeleted = true;
                role.ModifiedBy = currentUserId;
                role.ModifiedDate = DateTime.UtcNow;

                await _roleRepository.UpdateAsync(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role with ID: {RoleId}", id);
                throw;
            }
        }
    }
}