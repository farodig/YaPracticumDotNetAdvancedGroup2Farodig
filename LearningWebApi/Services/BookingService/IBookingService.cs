using LearningWebApi.Entities;

namespace LearningWebApi.Services.BookingService
{
    /// <summary>
    /// Сервис бронирования
    /// </summary>
    public interface IBookingService
    {
        /// <summary>
        /// создание брони для указанного события
        /// </summary>
        public Booking? CreateBooking(Guid eventId);

        /// <summary>
        /// получение брони по идентификатору
        /// </summary>
        public Booking? GetBookingById(Guid bookingId);
    }
}
