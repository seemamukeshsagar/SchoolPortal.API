using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Interfaces.Repositories
{
    public interface ISchoolContactRepository : IRepository<SchoolContactMaster>
    {
        Task<IEnumerable<SchoolContactMaster>> GetBySchoolIdWithDetailsAsync(Guid schoolId);
        Task<SchoolContactMaster> GetByIdWithDetailsAsync(Guid id);
    }
}
