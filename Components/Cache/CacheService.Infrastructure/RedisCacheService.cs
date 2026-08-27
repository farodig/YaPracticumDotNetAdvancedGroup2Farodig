using CacheService.Application;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace CacheService.Infrastructure
{
    public class RedisCacheService<TItem>(IConnectionMultiplexer connectionMultiplexer, IOptions<RedisCacheSettings> options, ILogger<RedisCacheService<object>> logger) : ICacheService<TItem>
        where TItem : class
    {
        private readonly TimeSpan _generalTimeToLive = TimeSpan.FromSeconds(options.Value.GeneralTtlSec);
        private readonly ILogger<RedisCacheService<object>> _logger = logger;
        private readonly string _keyType = typeof(TItem).Name.ToLowerInvariant();

        public async Task<TItem?> GetAsync(Guid id)
        {
            var key = GetKey(id);
            return await GetInternalAsync<TItem>(key);
        }

        public async Task<IEnumerable<TItem>?> GetCollectionAsync(string key)
        {
            var combinedKey = GetKey(key);
            return await GetInternalAsync<List<TItem>>(combinedKey);
        }

        public async Task SetAsync(Guid id, TItem item, TimeSpan? timeToLive = null)
        {
            var key = GetKey(id);
            await SetInternalAsync(key, item, timeToLive);
        }

        public async Task SetCollectionAsync(string key, IEnumerable<TItem> collection, TimeSpan? timeToLive = null)
        {
            var combinedKey = GetKey(key);
            await SetInternalAsync(combinedKey, collection.ToList(), timeToLive);
        }

        public async Task DeleteAsync(Guid id)
        {
            var key = GetKey(id);

            try
            {
                var db = GetDatabase();
                await db.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unable to delete item from cache by {key}", key);
            }
        }

        private async Task SetInternalAsync<T>(string key, T item, TimeSpan? timeToLive = null)
        {
            try
            {
                var value = JsonSerializer.Serialize(item);

                var db = GetDatabase();
                await db.StringSetAsync(key, value, timeToLive ?? _generalTimeToLive);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unable to set value to cache by {key}", key);
            }
        }

        private async Task<T?> GetInternalAsync<T>(string key)
        {
            try
            {
                var db = GetDatabase();
                var result = await db.StringGetAsync(key);

                if (result.IsNullOrEmpty)
                {
                    return default;
                }

                if (JsonSerializer.Deserialize<T>(result.ToString()) is not T value)
                {
                    _logger.LogError("Unable to deserialize {key}", key);
                    return default;
                }

                return value;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unable to get item from cache by {key}", key);
                return default;
            }
        }

        private IDatabase GetDatabase()
        {
            return connectionMultiplexer.GetDatabase();
        }

        private string GetKey(Guid id)
        {
            return $"{_keyType}:{id}";
        }

        private string GetKey(string suffix)
        {
            return $"{_keyType}:{suffix}";
        }
    }
}
