using Microsoft.EntityFrameworkCore;
using SchoolPortal.API.Data;
using SchoolPortal.API.Interfaces.Repositories;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Repositories
{
    public class ClassRepository : Repository<ClassMaster>, IClassRepository
    {
        public ClassRepository(SchoolPortalNewContext context) : base(context)
        {
        }

        // Implement any class-specific repository methods here
    }
}