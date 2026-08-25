using CacheService.Application;
using Microsoft.Extensions.Options;
using NLog;
using StackExchange.Redis;
using System.Text.Json;

namespace CacheService.Infrastructure
{
    public class RedisCacheService<TItem>(IConnectionMultiplexer connectionMultiplexer, IOptions<RedisCacheSettings> options) : ICacheService<TItem>
        where TItem : class
    {
        private readonly TimeSpan _generalTimeToLive = TimeSpan.FromSeconds(options.Value.GeneralTtlSec);
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public TItem? Get(string key)
        {
            try
            {
                var result = GetDatabase()
                    .StringGet(key);

                if (result.IsNullOrEmpty)
                {
                    return default;
                }

                if (JsonSerializer.Deserialize<TItem>(result.ToString()) is not TItem value)
                {
                    _logger.Error($"Unable to deserialize {typeof(TItem).Name}");
                    return default;
                }

                return value;
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Unable to get item from cache by key {key}", key);
                return default;
            }
        }

        public void Set(string key, TItem item, TimeSpan? timeToLive)
        {
            try
            {
                var value = JsonSerializer.Serialize(item);

                GetDatabase()
                    .StringSet(key, value, timeToLive ?? _generalTimeToLive);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Unable to set value to cache by key {key}", key);
            }
        }

        public void Delete(string key)
        {
            try
            {
                GetDatabase()
                    .KeyDelete(key);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Unable to delete item from cache by key {key}", key);
            }
        }

        private IDatabase GetDatabase()
        {
            return connectionMultiplexer.GetDatabase();
        }
    }
}
