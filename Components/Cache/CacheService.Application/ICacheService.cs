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
        TItem? Get(string key);

        /// <summary>
        /// Записать с временем жизни
        /// </summary>
        void Set(string key, TItem item, TimeSpan? expired);

        /// <summary>
        /// Удалить по ключу
        /// </summary>
        void Delete(string key);
    }
}
