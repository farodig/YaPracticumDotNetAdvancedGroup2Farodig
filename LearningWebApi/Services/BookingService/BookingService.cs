using LearningWebApi.Entities;
using LearningWebApi.Entities.Factories;
using LearningWebApi.Repositories;
using System.Collections.Concurrent;

namespace LearningWebApi.Services.BookingService
{
    internal class BookingService(IBookingRepository repository) : IBookingService
    {
        private IBookingRepository _repository = repository;
        private readonly ConcurrentDictionary<Guid, Guid> _mapEventToBooking = new();

        public async ValueTask<Booking> CreateBookingAsync(Guid eventId)
        {
            // одно событие можно забронировать только один раз
            if (_mapEventToBooking.TryGetValue(eventId, out Guid bookingId)
                && await GetBookingByIdAsync(bookingId) is Booking cached)
            {
                return cached;
            }

            var booking = BookingFactory.CreateBooking(eventId);
            _repository.Add(booking.Id, booking);
            _mapEventToBooking[eventId] = booking.Id;
            return booking;
        }

        public async ValueTask<Booking?> GetBookingByIdAsync(Guid id)
        {
            _repository.TryGetValue(id, out Booking? item);
            return item;
        }
    }
}
