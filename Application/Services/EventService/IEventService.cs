using Application.Models.Requests;
using Application.Models.Responses;

namespace Application.Services.EventService
{
    /// <summary>
    /// Сервис событий
    /// </summary>
    public interface IEventService
    {
        /// <summary>
        /// Получить все события постранично и с фильтрами
        /// </summary>
        Task<IEnumerable<EventResponse>> GetEventsAsync(
            int page,
            int pageSize,
            string? title = null, 
            DateTime? from = null, 
            DateTime? to = null, 
            CancellationToken cts = default);

        /// <summary>
        /// Получить событие по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор события</param>
        /// <param name="cts">Токен отмены</param>
        Task<EventResponse?> GetEventAsync(Guid id, CancellationToken cts = default);

        /// <summary>
        /// Создать событие
        /// </summary>
        /// <param name="title">Заголовок события</param>
        /// <param name="startAt">Время начала события</param>
        /// <param name="endAt">Время окончания события</param>
        /// <param name="description">Описание события</param>
        /// <param name="cts">Токен отмены</param>
        /// <param name="totalSeats">Общее количество мест на событии</param>
        Task<EventResponse> CreateEventAsync(string title, DateTime startAt, DateTime endAt, int totalSeats, string? description = null, CancellationToken cts = default);

        /// <summary>
        /// Обновить существующее событие
        /// </summary>
        /// <param name="item">Событие</param>
        /// <param name="cts">Токен отмены</param>
        /// <returns>true если удалось совершить действие над событием, false если событие не найдено</returns>
        Task<bool> TryUpdateEventAsync(Guid id, UpdateEventRequest item, CancellationToken cts = default);

        /// <summary>
        /// Удалить существующее событие
        /// </summary>
        /// <param name="id">Идентификатор события</param>
        /// <param name="cts">Токен отмены</param>
        /// <returns>true если удалось совершить действие над событием, false если событие не найдено</returns>
        Task<bool> TryDeleteEventAsync(Guid id, CancellationToken cts = default);

        /// <summary>
        /// Зарезерировать место на событии
        /// </summary>
        Task ReserveSeatAsync(Guid id, CancellationToken cts = default);

        /// <summary>
        /// Освободить место на событии
        /// </summary>
        Task ReleaseSeatAsync(Guid id, CancellationToken cts = default);
    }
}
