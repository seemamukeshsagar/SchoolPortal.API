// SchoolPortal.API/Interfaces/Repositories/IClassSubjectDetailRepository.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Interfaces.Repositories
{
    public interface IClassSubjectDetailRepository
    {
        Task<IEnumerable<ClassSubjectDetail>> GetAllAsync();
        Task<ClassSubjectDetail> GetByIdAsync(Guid id);
        Task AddAsync(ClassSubjectDetail classSubjectDetail);
        void Update(ClassSubjectDetail classSubjectDetail);
        void Delete(ClassSubjectDetail classSubjectDetail);
    }
}