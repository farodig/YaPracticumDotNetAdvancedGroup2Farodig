namespace CacheService.Infrastructure
{
    /// <summary>
    /// Настройки кеширования
    /// </summary>
    public class RedisCacheSettings
    {
        /// <summary>
        /// Строка подключения к redis
        /// </summary>
        public string ConnectionString { get; set; } = "localhost:6379";

        /// <summary>
        /// Общее время таймаутов от кеша
        /// </summary>
        public int TimeoutMs { get; set; } = 200;

        /// <summary>
        /// Общее время хранения записи кеша, сек
        /// </summary>
        public int GeneralTtlSec = 5;
    }
}
