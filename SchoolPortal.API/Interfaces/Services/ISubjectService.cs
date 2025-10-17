// Interfaces/Services/ISubjectService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolPortal.API.DTOs.Subject;

namespace SchoolPortal.API.Interfaces.Services
{
    public interface ISubjectService
    {
        Task<IEnumerable<SubjectDto>> GetAllSubjectsAsync();
        Task<SubjectDto> GetSubjectByIdAsync(Guid id);
        Task<SubjectDto> CreateSubjectAsync(CreateSubjectRequest request);
        Task<SubjectDto> UpdateSubjectAsync(UpdateSubjectRequest request);
        Task<bool> DeleteSubjectAsync(Guid id);
        Task<IEnumerable<SubjectDto>> GetSubjectsBySchoolAsync(Guid schoolId);
    }
}