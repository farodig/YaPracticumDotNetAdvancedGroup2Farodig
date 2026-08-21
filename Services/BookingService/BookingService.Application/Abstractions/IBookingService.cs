using BookingService.Application.Models.Responses;
using BookingService.Domain.Entities;

namespace BookingService.Application.Abstractions
{
    /// <summary>
    /// Сервис бронирования
    /// </summary>
    public interface IBookingService
    {
        /// <summary>
        /// Максимальное допустимое количество активных бронирований у пользователя
        /// </summary>
        public const int PersonMaxBookingCount = 10;

        /// <summary>
        /// Создание брони для указанного события
        /// </summary>
        Task<BookingResponse> CreateBookingAsync(Guid eventId, Guid personId, CancellationToken cts = default);

        /// <summary>
        /// Получение брони по идентификатору
        /// </summary>
        Task<BookingResponse> GetBookingByIdAsync(Guid id, CancellationToken cts = default);

        /// <summary>
        /// Отменить бронирование
        /// </summary>
        Task CancelBookingAsync(Booking data, CancellationToken cts = default);

        /// <summary>
        /// Администратор отменяет бронь
        /// </summary>
        Task CancelBookingByAdminAsync(Guid bookingId, CancellationToken cts = default);

        /// <summary>
        /// Бронь отменяет пользователь
        /// </summary>
        Task CancelBookingByPersonAsync(Guid bookingId, Guid personId, CancellationToken cts = default);

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
