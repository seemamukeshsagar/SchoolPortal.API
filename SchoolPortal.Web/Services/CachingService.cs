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
            if (string.IsNullOrEmpty(cacheKey))
            {
                throw new ArgumentNullException(nameof(cacheKey));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (_cache.TryGetValue(cacheKey, out T? cachedValue) && cachedValue != null)
            {
                _logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
                return cachedValue;
            }

            _logger.LogDebug("Cache miss for key: {CacheKey}", cacheKey);
            var value = await factory().ConfigureAwait(false);

            if (value == null)
            {
                throw new InvalidOperationException("Factory method returned a null value.");
            }

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpiration ?? TimeSpan.FromMinutes(30)
            };

            _cache.Set(cacheKey, value, cacheOptions);
            return value;
        }

        public T? Get<T>(string cacheKey) where T : class
        {
            if (string.IsNullOrEmpty(cacheKey))
            {
                _logger.LogWarning("Null or empty cache key provided");
                return null;
            }

            return _cache.Get<T>(cacheKey);
        }

        public void Set<T>(string cacheKey, T value, TimeSpan? absoluteExpiration = null) where T : class
        {
            if (string.IsNullOrEmpty(cacheKey))
            {
                _logger.LogWarning("Cannot set cache with null or empty key");
                return;
            }

            if (value == null)
            {
                _logger.LogWarning("Cannot cache null value for key: {CacheKey}", cacheKey);
                return;
            }

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpiration ?? TimeSpan.FromMinutes(30)
            };
            _cache.Set(cacheKey, value, cacheOptions);
            _logger.LogDebug("Cached value for key: {CacheKey}", cacheKey);
        }

        public void Remove(string cacheKey)
        {
            if (string.IsNullOrEmpty(cacheKey))
            {
                _logger.LogWarning("Cannot remove cache with null or empty key");
                return;
            }

            _cache.Remove(cacheKey);
            _logger.LogDebug("Cache removed for key: {CacheKey}", cacheKey);
        }
    }
}