using System;
using System.Threading.Tasks;
using SchoolPortal.API.Interfaces;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly SchoolPortalNewContext _context;
        private bool _disposed = false;

        public UnitOfWork(SchoolPortalNewContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IRepository<T> GetRepository<T>() where T : class
        {
            return new Repository<T>(_context);
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
