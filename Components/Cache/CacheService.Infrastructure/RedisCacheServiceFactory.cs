using CacheService.Application;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace CacheService.Infrastructure
{
    /// <summary>
    /// Фабрика создания сервиса кеширования
    /// </summary>
    public class RedisCacheServiceFactory(IConnectionMultiplexer connectionMultiplexer, IOptions<RedisCacheSettings> options, ILogger<RedisCacheService<object>> logger) : ICacheServiceFactory
    {
        public ICacheService<TItem> CreateCacheService<TItem>()
            where TItem : class
        {
            return new RedisCacheService<TItem>(connectionMultiplexer, options, logger);
        }
    }
}
