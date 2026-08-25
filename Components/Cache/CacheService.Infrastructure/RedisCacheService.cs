using CacheService.Application;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace CacheService.Infrastructure
{
    public class RedisCacheService<TItem>(IConnectionMultiplexer connectionMultiplexer, IOptions<RedisCacheSettings> options) : ICacheService<TItem>
    {
        private readonly RedisCacheSettings settings = options.Value;

        public bool Delete(string key, out TItem item)
        {
            throw new NotImplementedException();
        }

        public TItem Get(string key)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, TItem item, TimeSpan expired)
        {
            throw new NotImplementedException();
        }
    }
}
