using Domain.Entities;

namespace LearningWebApi.Repositories
{
    /// <summary>
    /// Репозиторий бронирований
    /// </summary>
    public interface IBookingRepository
    {
        /// <summary>
        /// Получить бронь по идентификатору
        /// </summary>
        Task<Booking?> GetAsync(Guid id, CancellationToken cts = default);

        /// <summary>
        /// Получить все бронирования
        /// </summary>
        Task<IEnumerable<Booking>> GetBookingsByStatus(BookingStatus status, CancellationToken cts = default);

        /// <summary>
        /// Создать бронь события
        /// </summary>
        Task CreateAsync(Booking item, CancellationToken cts = default);

        /// <summary>
        /// Обновить бронь события
        /// </summary>
        Task<int> TryUpdateAsync(Booking item, CancellationToken cts = default);

        /// <summary>
        /// Удалить бронирование
        /// </summary>
        Task<int> TryRemoveAsync(Guid id, CancellationToken cts = default);
    }
}
