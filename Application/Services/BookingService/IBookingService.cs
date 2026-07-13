using Domain.Entities;

namespace Application.Services.BookingService
{
    /// <summary>
    /// Сервис бронирования
    /// </summary>
    public interface IBookingService
    {
        /// <summary>
        /// Создание брони для указанного события
        /// </summary>
        Task<Booking> CreateBookingAsync(Guid eventId, CancellationToken cts = default);

        /// <summary>
        /// Получение брони по идентификатору
        /// </summary>
        Task<Booking?> GetBookingByIdAsync(Guid id, CancellationToken cts = default);

        /// <summary>
        /// Отменить бронирование
        /// </summary>
        Task CancelBookingAsync(Booking data, CancellationToken cts = default);

        /// <summary>
        /// Получить необработанные бронирования
        /// </summary>
        Task<IEnumerable<Booking>> GetPendingByCreatedAsync(CancellationToken cts = default);

        /// <summary>
        /// Подтвердить бронь
        /// </summary>
        Task ConfirmBookingAsync(Booking data, CancellationToken cts = default);

        /// <summary>
        /// Отклонить бронь
        /// </summary>
        Task RejectBookingAsync(Booking data, CancellationToken cts = default);
    }
}
