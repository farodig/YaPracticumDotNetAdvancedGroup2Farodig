namespace CacheService.Application
{
    /// <summary>
    /// Сервис кеширования
    /// </summary>
    public interface ICacheService<TItem>
    {
        /// <summary>
        /// Получить значение по ключу
        /// </summary>
        TItem Get(string key);

        /// <summary>
        /// Записать с временем жизни
        /// </summary>
        void Set(string key, TItem item, TimeSpan expired);

        /// <summary>
        /// Удалить по ключу
        /// </summary>
        bool Delete(string key, out TItem item);
    }
}
