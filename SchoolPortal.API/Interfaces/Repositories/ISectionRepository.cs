using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Interfaces.Repositories
{
    public interface ISectionRepository
    {
        Task<IEnumerable<SectionMaster>> GetAllSectionsAsync();
        Task<SectionMaster?> GetSectionByIdAsync(Guid id);
        Task<SectionMaster> CreateSectionAsync(SectionMaster section);
        Task<SectionMaster> UpdateSectionAsync(SectionMaster section);
        Task<bool> DeleteSectionAsync(Guid id);
    }
}