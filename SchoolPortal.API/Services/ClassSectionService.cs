// SchoolPortal.API/Services/ClassSectionService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using SchoolPortal.API.DTOs.ClassSection;
using SchoolPortal.API.Interfaces.Repositories;
using SchoolPortal.API.Interfaces.Services;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Services
{
    public class ClassSectionService : IClassSectionService
    {
        private readonly IClassSectionRepository _classSectionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ClassSectionService> _logger;

        public ClassSectionService(
            IClassSectionRepository classSectionRepository,
            IMapper mapper,
            ILogger<ClassSectionService> logger)
        {
            _classSectionRepository = classSectionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ClassSectionDetailDto>> GetAllClassSectionsAsync()
        {
            try
            {
                var classSections = await _classSectionRepository.GetAllClassSectionsAsync();
                return _mapper.Map<IEnumerable<ClassSectionDetailDto>>(classSections);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all class sections");
                throw;
            }
        }

        public async Task<ClassSectionDetailDto> GetClassSectionByIdAsync(Guid id)
        {
            try
            {
                var classSection = await _classSectionRepository.GetClassSectionByIdAsync(id);
                if (classSection == null)
                {
                    return null;
                }
                return _mapper.Map<ClassSectionDetailDto>(classSection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving class section with ID: {ClassSectionId}", id);
                throw;
            }
        }

        public async Task<ClassSectionDetailDto> CreateClassSectionAsync(CreateClassSectionDetailRequest request)
        {
            try
            {
                var classSection = _mapper.Map<ClassSectionDetail>(request);
                var createdClassSection = await _classSectionRepository.CreateClassSectionAsync(classSection);
                return _mapper.Map<ClassSectionDetailDto>(createdClassSection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating class section");
                throw;
            }
        }

        public async Task<ClassSectionDetailDto> UpdateClassSectionAsync(UpdateClassSectionDetailRequest request)
        {
            try
            {
                var existingClassSection = await _classSectionRepository.GetClassSectionByIdAsync(request.Id);
                if (existingClassSection == null)
                {
                    return null;
                }

                _mapper.Map(request, existingClassSection);
                var updatedClassSection = await _classSectionRepository.UpdateClassSectionAsync(existingClassSection);
                return _mapper.Map<ClassSectionDetailDto>(updatedClassSection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating class section with ID: {ClassSectionId}", request.Id);
                throw;
            }
        }

        public async Task<bool> DeleteClassSectionAsync(Guid id)
        {
            try
            {
                return await _classSectionRepository.DeleteClassSectionAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting class section with ID: {ClassSectionId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<ClassSectionDetailDto>> GetClassSectionsByClassIdAsync(Guid classId)
        {
            try
            {
                var allClassSections = await _classSectionRepository.GetAllClassSectionsAsync();
                var filteredSections = allClassSections.Where(cs => cs.ClassId == classId);
                return _mapper.Map<IEnumerable<ClassSectionDetailDto>>(filteredSections);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving class sections for class ID: {ClassId}", classId);
                throw;
            }
        }

        public async Task<IEnumerable<ClassSectionDetailDto>> GetActiveClassSectionsAsync()
        {
            try
            {
                var allClassSections = await _classSectionRepository.GetAllClassSectionsAsync();
                var activeSections = allClassSections.Where(cs => cs.IsActive);
                return _mapper.Map<IEnumerable<ClassSectionDetailDto>>(activeSections);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active class sections");
                throw;
            }
        }
    }
}