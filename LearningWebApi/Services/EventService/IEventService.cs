using LearningWebApi.Entities;

namespace LearningWebApi.Services.EventService
{
    /// <summary>
    /// Сервис событий
    /// </summary>
    public interface IEventService
    {
        /// <summary>
        /// Получить все события
        /// </summary>
        IQueryable<Event> GetEvents();

        /// <summary>
        /// Получить событие по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор события</param>
        /// <param name="cts">Токен отмены</param>
        Task<Event?> GetEventAsync(Guid id, CancellationToken? cts = null);

        /// <summary>
        /// Создать событие
        /// </summary>
        /// <param name="title">Заголовок события</param>
        /// <param name="startAt">Время начала события</param>
        /// <param name="endAt">Время окончания события</param>
        /// <param name="description">Описание события</param>
        /// <param name="cts">Токен отмены</param>
        /// <param name="totalSeats">Общее количество мест на событии</param>
        Task<Event> CreateEventAsync(string title, DateTime startAt, DateTime endAt, int totalSeats, string? description = null, CancellationToken? cts = null);

        /// <summary>
        /// Обновить существующее событие
        /// </summary>
        /// <param name="item">Событие</param>
        /// <param name="cts">Токен отмены</param>
        /// <returns>true если удалось совершить действие над событием, false если событие не найдено</returns>
        Task<bool> TryUpdateEventAsync(Event item, CancellationToken? cts = null);

        /// <summary>
        /// Удалить существующее событие
        /// </summary>
        /// <param name="id">Идентификатор события</param>
        /// <param name="cts">Токен отмены</param>
        /// <returns>true если удалось совершить действие над событием, false если событие не найдено</returns>
        Task<bool> TryDeleteEventAsync(Guid id, CancellationToken? cts = null);

        /// <summary>
        /// Зарезерировать место на событии
        /// </summary>
        Task ReserveSeatAsync(Guid id, CancellationToken? cts = null);

        /// <summary>
        /// Освободить место на событии
        /// </summary>
        Task ReleaseSeatAsync(Guid id, CancellationToken? cts = null);
    }
}
