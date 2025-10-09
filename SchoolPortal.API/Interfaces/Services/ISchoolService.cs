using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolPortal.API.DTOs.School;

namespace SchoolPortal.API.Interfaces.Services
{
    public interface ISchoolService
    {
        Task<IEnumerable<SchoolDto>> GetAllSchoolsAsync();
        Task<SchoolDto> GetSchoolByIdAsync(Guid id);
        Task<SchoolDto> CreateSchoolAsync(CreateSchoolRequest request);
        Task<SchoolDto> UpdateSchoolAsync(UpdateSchoolRequest request);
        Task<bool> DeleteSchoolAsync(Guid id);
        Task<IEnumerable<SchoolDto>> GetSchoolsByCompanyIdAsync(Guid companyId);
    }
}
