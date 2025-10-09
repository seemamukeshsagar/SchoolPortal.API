using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Interfaces.Repositories
{
    public interface ISchoolRepository
    {
        Task<IEnumerable<SchoolMaster>> GetAllSchoolsAsync();
        Task<SchoolMaster> GetSchoolByIdAsync(Guid id);
        Task<SchoolMaster> CreateSchoolAsync(SchoolMaster school);
        Task<SchoolMaster> UpdateSchoolAsync(SchoolMaster school);
        Task<bool> DeleteSchoolAsync(Guid id);
        Task<IEnumerable<SchoolMaster>> GetSchoolsByCompanyIdAsync(Guid companyId);
    }
}
