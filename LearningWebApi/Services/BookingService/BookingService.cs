using LearningWebApi.Models;
using LearningWebApi.Repositories;

namespace LearningWebApi.Services.BookingService
{
    internal class BookingService(IBookingRepository repository) : IBookingService
    {
        private IBookingRepository _repository = repository;

        public async ValueTask<Booking> CreateBookingAsync(Guid eventId)
        {
            var booking = BookingFactory.CreateBooking(eventId);
            _repository.Add(booking.Id, booking);
            return booking;
        }

        public async ValueTask<Booking?> GetBookingByIdAsync(Guid id)
        {
            _repository.TryGetValue(id, out Booking? item);
            return item;
        }
    }
}
