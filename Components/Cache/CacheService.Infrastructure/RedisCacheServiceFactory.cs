using CacheService.Application;
using Microsoft.Extensions.Options;

namespace CacheService.Infrastructure
{
    /// <summary>
    /// Фабрика создания сервиса кеширования
    /// </summary>
    public class RedisCacheServiceFactory(IOptions<RedisCacheSettings> options) : ICacheServiceFactory
    {
        private readonly IOptions<RedisCacheSettings> _options = options;
        public ICacheService<TItem> CreateCacheService<TItem>()
        {
            return new RedisCacheService<TItem>(_options);
        }
    }
}
