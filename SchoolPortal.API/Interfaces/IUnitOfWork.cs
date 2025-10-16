using System;
using System.Threading.Tasks;

namespace SchoolPortal.API.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> GetRepository<T>() where T : class;
        Task<int> CommitAsync();
    }
}
