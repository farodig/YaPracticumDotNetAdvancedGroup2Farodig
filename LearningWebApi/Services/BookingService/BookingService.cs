using LearningWebApi.Entities;
using LearningWebApi.Entities.Factories;
using LearningWebApi.Repositories;
using NLog;

namespace LearningWebApi.Services.BookingService
{
    internal class BookingService(IEventRepository eventRepository, IBookingRepository bookingRepository) : IBookingService
    {
        private readonly IEventRepository _eventRepository = eventRepository;
        private readonly IBookingRepository _bookingRepository = bookingRepository;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public async ValueTask<Booking?> CreateBookingAsync(Guid eventId)
        {
            if (!_eventRepository.ContainsKey(eventId))
            {
                // Нельзя забронировать, т. к. событие не найдено
                return null;
            }

            var booking = BookingFactory.CreateBooking(eventId);
            _bookingRepository.Add(booking.Id, booking);
            _logger.Info($"Booking #{booking.Id} created with status '{booking.Status}'");
            return booking;
        }

        public async ValueTask<Booking?> GetBookingByIdAsync(Guid id)
        {
            _bookingRepository.TryGetValue(id, out Booking? item);
            return item;
        }
    }
}
