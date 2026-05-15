using LearningWebApi.Entities;
using LearningWebApi.Entities.Factories;
using LearningWebApi.Repositories;

namespace LearningWebApi.Services.BookingService
{
    internal class BookingService(IEventRepository eventRepository, IBookingRepository bookingRepository) : IBookingService
    {
        private readonly IEventRepository _eventRepository = eventRepository;
        private readonly IBookingRepository _bookingRepository = bookingRepository;

        public async ValueTask<Booking?> CreateBookingAsync(Guid eventId)
        {
            if (!_eventRepository.ContainsKey(eventId))
            {
                // Нельзя забронировать, т. к. событие не найдено
                return null;
            }

            var booking = BookingFactory.CreateBooking(eventId);
            _bookingRepository.Add(booking.Id, booking);
            return booking;
        }

        public async ValueTask<Booking?> GetBookingByIdAsync(Guid id)
        {
            _bookingRepository.TryGetValue(id, out Booking? item);
            return item;
        }
    }
}
