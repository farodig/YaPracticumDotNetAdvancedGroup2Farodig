using EventService.Domain.Entities;

namespace EventService.Application.Abstractions
{
    /// <summary>
    /// Репозиторий событий
    /// </summary>
    public interface IEventRepository
    {
        /// <summary>
        /// Получить все события постранично и с фильтрами
        /// </summary>
        Task<IEnumerable<Event>> GetEventsAsync(
            int page,
            int pageSize,
            string? title = null,
            DateTime? from = null,
            DateTime? to = null,
            CancellationToken cts = default);

        /// <summary>
        /// Получить событие по идентификатору
        /// </summary>
        Task<Event?> GetAsync(Guid id, CancellationToken cts = default);

        /// <summary>
        /// Создать событие
        /// </summary>
        Task CreateAsync(Event item, CancellationToken cts = default);

        /// <summary>
        /// Обновить событие
        /// </summary>
        /// <returns>Количество удалённых из базы событий</returns>
        Task<int> TryUpdateAsync(Event item, CancellationToken cts = default);

        /// <summary>
        /// Удалить событие по Id
        /// </summary>
        /// <returns>Количество удалённых из базы событий</returns>
        Task<int> TryRemoveAsync(Guid id, CancellationToken cts = default);

        /// <summary>
        /// Паттерн inbox
        /// </summary>
        /// <returns></returns>
        Task<bool> IsInboxDublicatedEvent(Guid id, CancellationToken cts = default);
    }
}
