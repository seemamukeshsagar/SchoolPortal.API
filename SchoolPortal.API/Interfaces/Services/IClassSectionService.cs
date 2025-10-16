// SchoolPortal.API/Interfaces/Services/IClassSectionService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolPortal.API.DTOs.ClassSection;

namespace SchoolPortal.API.Interfaces.Services
{
    public interface IClassSectionService
    {
        Task<IEnumerable<ClassSectionDetailDto>> GetAllClassSectionsAsync();
        Task<ClassSectionDetailDto> GetClassSectionByIdAsync(Guid id);
        Task<ClassSectionDetailDto> CreateClassSectionAsync(CreateClassSectionDetailRequest request);
        Task<ClassSectionDetailDto> UpdateClassSectionAsync(UpdateClassSectionDetailRequest request);
        Task<bool> DeleteClassSectionAsync(Guid id);
        Task<IEnumerable<ClassSectionDetailDto>> GetClassSectionsByClassIdAsync(Guid classId);
        Task<IEnumerable<ClassSectionDetailDto>> GetActiveClassSectionsAsync();
    }
}