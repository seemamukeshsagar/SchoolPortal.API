using Microsoft.Extensions.Caching.Memory;

namespace SchoolPortal.Web.Services
{
    public interface ICachingService
    {
        Task<T> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> factory, TimeSpan? absoluteExpiration = null);
        void Remove(string cacheKey);
        T? Get<T>(string cacheKey) where T : class;
        void Set<T>(string cacheKey, T value, TimeSpan? absoluteExpiration = null) where T : class;
    }
}