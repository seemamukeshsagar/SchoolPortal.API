// Interfaces/Repositories/ISubjectRepository.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Interfaces.Repositories
{
    public interface ISubjectRepository
    {
        Task<IEnumerable<SubjectMaster>> GetAllSubjectsAsync();
        Task<SubjectMaster> GetSubjectByIdAsync(Guid id);
        Task<SubjectMaster> CreateSubjectAsync(SubjectMaster subject);
        Task<SubjectMaster> UpdateSubjectAsync(SubjectMaster subject);
        Task<bool> DeleteSubjectAsync(Guid id);
        Task<IEnumerable<SubjectMaster>> GetSubjectsBySchoolAsync(Guid schoolId);
    }
}