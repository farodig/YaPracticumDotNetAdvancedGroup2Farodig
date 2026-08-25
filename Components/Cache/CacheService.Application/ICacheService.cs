namespace CacheService.Application
{
    /// <summary>
    /// Сервис кеширования
    /// </summary>
    public interface ICacheService<TItem>
        where TItem : class
    {
        /// <summary>
        /// Получить значение по ключу
        /// </summary>
        Task<TItem?> GetAsync(Guid id);

        /// <summary>
        /// Получить коллекцию по ключу
        /// </summary>
        Task<IEnumerable<TItem>?> GetCollectionAsync(string key);

        /// <summary>
        /// Записать с временем жизни
        /// </summary>
        Task SetAsync(Guid id, TItem item, TimeSpan? timeToLive = null);

        /// <summary>
        /// Записать коллекцию
        /// </summary>
        Task SetCollectionAsync(string key, IEnumerable<TItem> collection, TimeSpan? timeToLive = null);

        /// <summary>
        /// Удалить по ключу
        /// </summary>
        Task DeleteAsync(Guid id);
    }
}
