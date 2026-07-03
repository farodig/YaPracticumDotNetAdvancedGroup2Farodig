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
        /// получение брони по идентификатору
        /// </summary>
        public Booking? GetBookingById(Guid bookingId);

        /// <summary>
        /// Отменить бронирование
        /// </summary>
        public void CancelBooking(Guid bookingId);

        /// <summary>
        /// Получить необработанные бронирования
        /// </summary>
        IEnumerable<Booking> GetPending();

        /// <summary>
        /// Подтвердить бронь
        /// </summary>
        void ConfirmBooking(Booking data);

        /// <summary>
        /// Отклонить бронь
        /// </summary>
        Task RejectBookingAsync(Booking data, CancellationToken? cts = null);
    }
}
