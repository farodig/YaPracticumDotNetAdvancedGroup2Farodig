using CacheService.Application;
using Microsoft.Extensions.Options;

namespace CacheService.Infrastructure
{
    public class RedisCacheService<TItem> : ICacheService<TItem>
    {
        public RedisCacheService(IOptions<RedisCacheSettings> options)
        {
        }

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
