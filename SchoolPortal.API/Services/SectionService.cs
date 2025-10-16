using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using SchoolPortal.API.DTOs.Section;
using SchoolPortal.API.Interfaces.Repositories;
using SchoolPortal.API.Interfaces.Services;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Services
{
    public class SectionService : ISectionService
    {
        private readonly ISectionRepository _sectionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<SectionService> _logger;

        public SectionService(
            ISectionRepository sectionRepository,
            IMapper mapper,
            ILogger<SectionService> logger)
        {
            _sectionRepository = sectionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SectionListDto>> GetAllSectionsAsync()
        {
            try
            {
                var sections = await _sectionRepository.GetAllSectionsAsync();
                return _mapper.Map<IEnumerable<SectionListDto>>(sections);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all sections");
                throw;
            }
        }

        public async Task<SectionDto?> GetSectionByIdAsync(Guid id)
        {
            try
            {
                var section = await _sectionRepository.GetSectionByIdAsync(id);
                if (section == null)
                {
                    return null;
                }
                return _mapper.Map<SectionDto>(section);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching section with id: {id}");
                throw;
            }
        }

        public async Task<SectionDto> CreateSectionAsync(CreateSectionRequest request)
        {
            try
            {
                var section = _mapper.Map<SectionMaster>(request);
                section.Id = Guid.NewGuid();
                
                var createdSection = await _sectionRepository.CreateSectionAsync(section);
                return _mapper.Map<SectionDto>(createdSection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating section");
                throw;
            }
        }

        public async Task<bool> UpdateSectionAsync(Guid id, UpdateSectionRequest request)
        {
            try
            {
                var section = await _sectionRepository.GetSectionByIdAsync(id);
                if (section == null)
                {
                    return false;
                }

                _mapper.Map(request, section);
                await _sectionRepository.UpdateSectionAsync(section);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating section with id: {id}");
                throw;
            }
        }

        public async Task<bool> DeleteSectionAsync(Guid id)
        {
            try
            {
                return await _sectionRepository.DeleteSectionAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting section with id: {id}");
                throw;
            }
        }
    }
}