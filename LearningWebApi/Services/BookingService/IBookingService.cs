using LearningWebApi.Entities;

namespace LearningWebApi.Services.BookingService
{
    /// <summary>
    /// Сервис бронирования
    /// </summary>
    public interface IBookingService
    {
        /// <summary>
        /// Создание брони для указанного события
        /// </summary>
        Task<Booking> CreateBookingAsync(Guid eventId, CancellationToken? cts = null);

        /// <summary>
        /// Получение брони по идентификатору
        /// </summary>
        Task<Booking?> GetBookingByIdAsync(Guid id, CancellationToken? cts = null);

        /// <summary>
        /// Отменить бронирование
        /// </summary>
        Task CancelBookingAsync(Booking data, CancellationToken? cts = null);

        /// <summary>
        /// Получить необработанные бронирования
        /// </summary>
        IEnumerable<Booking> GetPending();

        /// <summary>
        /// Подтвердить бронь
        /// </summary>
        Task ConfirmBookingAsync(Booking data, CancellationToken? cts = null);

        /// <summary>
        /// Отклонить бронь
        /// </summary>
        Task RejectBookingAsync(Booking data, CancellationToken? cts = null);
    }
}
