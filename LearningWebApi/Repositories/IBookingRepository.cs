using LearningWebApi.Entities;

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
        Task<Booking?> GetAsync(Guid id, CancellationToken? cts = null);

        /// <summary>
        /// Получить все бронирования
        /// </summary>
        IQueryable<Booking> GetBookings();

        /// <summary>
        /// Создать бронь события
        /// </summary>
        Task CreateAsync(Booking item, CancellationToken? cts = null);

        /// <summary>
        /// Обновить бронь события
        /// </summary>
        Task<int> TryUpdateAsync(Booking item, CancellationToken? cts = null);

        /// <summary>
        /// Удалить бронирование
        /// </summary>
        Task<int> TryRemoveAsync(Guid id, CancellationToken? cts = null);
    }
}
