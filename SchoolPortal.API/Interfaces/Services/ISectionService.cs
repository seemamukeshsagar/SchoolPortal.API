using SchoolPortal.API.DTOs.Section;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolPortal.API.Interfaces.Services
{
    public interface ISectionService
    {
        Task<IEnumerable<SectionListDto>> GetAllSectionsAsync();
        Task<SectionDto?> GetSectionByIdAsync(Guid id);
        Task<SectionDto> CreateSectionAsync(CreateSectionRequest request);
        Task<bool> UpdateSectionAsync(Guid id, UpdateSectionRequest request);
        Task<bool> DeleteSectionAsync(Guid id);
    }
}