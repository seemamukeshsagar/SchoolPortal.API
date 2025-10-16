using SchoolPortal.API.DTOs.Class;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolPortal.API.Interfaces.Services
{
    public interface IClassService
    {
        Task<IEnumerable<ClassListDto>> GetAllClassesAsync();
        Task<ClassDto> GetClassByIdAsync(Guid id);
        Task<ClassDto> CreateClassAsync(CreateClassRequest request, Guid userId);
        Task<ClassDto> UpdateClassAsync(Guid id, UpdateClassRequest request, Guid userId);
        Task<bool> DeleteClassAsync(Guid id, Guid userId);
    }
}