namespace LearningWebApi.Services.EventService
{
    /// <summary>
    /// Сервис событий
    /// </summary>
    public interface IEventService : IEventPagination
    {
        /// <summary>
        /// Получить все события
        /// </summary>
        IEnumerable<Event> GetEvents();

        /// <summary>
        /// Получить событие по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор события</param>
        Event? GetEvent(Guid id);

        /// <summary>
        /// Создать событие
        /// </summary>
        /// <param name="title">Заголовок события</param>
        /// <param name="startAt">Время начала события</param>
        /// <param name="endAt">Время окончания события</param>
        /// <param name="description">Описание события</param>
        Event CreateEvent(string title, DateTime startAt, DateTime endAt, string? description = null);

        /// <summary>
        /// Обновить существующее событие
        /// </summary>
        /// <param name="item">Событие</param>
        /// <returns>true если удалось совершить действие над событием, false если событие не найдено</returns>
        bool TryUpdateEvent(Event item);

        /// <summary>
        /// Удалить существующее событие
        /// </summary>
        /// <param name="id">Идентификатор события</param>
        /// <returns>true если удалось совершить действие над событием, false если событие не найдено</returns>
        bool TryDeleteEvent(Guid id);
    }
}
