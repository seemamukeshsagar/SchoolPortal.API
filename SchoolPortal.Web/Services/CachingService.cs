using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace SchoolPortal.Web.Services
{
    public class CachingService : ICachingService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<CachingService> _logger;

        public CachingService(IMemoryCache cache, ILogger<CachingService> logger)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<T> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> factory, TimeSpan? absoluteExpiration = null)
        {
            if (_cache.TryGetValue(cacheKey, out T cachedValue))
            {
                _logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
                return cachedValue;
            }

            _logger.LogDebug("Cache miss for key: {CacheKey}", cacheKey);
            var value = await factory();

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpiration ?? TimeSpan.FromMinutes(30)
            };

            _cache.Set(cacheKey, value, cacheOptions);
            return value;
        }

        public T Get<T>(string cacheKey)
        {
            return _cache.Get<T>(cacheKey);
        }

        public void Set<T>(string cacheKey, T value, TimeSpan? absoluteExpiration = null)
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpiration ?? TimeSpan.FromMinutes(30)
            };
            _cache.Set(cacheKey, value, cacheOptions);
        }

        public void Remove(string cacheKey)
        {
            _cache.Remove(cacheKey);
            _logger.LogDebug("Cache removed for key: {CacheKey}", cacheKey);
        }
    }
}