// SchoolPortal.API/Interfaces/Services/IClassSubjectDetailService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolPortal.API.DTOs.ClassSubjectDetail;

namespace SchoolPortal.API.Interfaces.Services
{
    public interface IClassSubjectDetailService
    {
        Task<IEnumerable<ClassSubjectDetailDto>> GetAllClassSubjectDetailsAsync();
        Task<ClassSubjectDetailDto> GetClassSubjectDetailByIdAsync(Guid id);
        Task<ClassSubjectDetailDto> CreateClassSubjectDetailAsync(CreateClassSubjectDetailRequest request);
        Task<ClassSubjectDetailDto> UpdateClassSubjectDetailAsync(Guid id, UpdateClassSubjectDetailRequest request);
        Task<bool> DeleteClassSubjectDetailAsync(Guid id);
    }
}