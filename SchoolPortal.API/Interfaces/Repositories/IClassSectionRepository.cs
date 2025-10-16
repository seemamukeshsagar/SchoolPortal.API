// SchoolPortal.API/Interfaces/Repositories/IClassSectionRepository.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Interfaces.Repositories
{
    public interface IClassSectionRepository
    {
        Task<IEnumerable<ClassSectionDetail>> GetAllClassSectionsAsync();
        Task<ClassSectionDetail> GetClassSectionByIdAsync(Guid id);
        Task<ClassSectionDetail> CreateClassSectionAsync(ClassSectionDetail classSection);
        Task<ClassSectionDetail> UpdateClassSectionAsync(ClassSectionDetail classSection);
        Task<bool> DeleteClassSectionAsync(Guid id);
    }
}