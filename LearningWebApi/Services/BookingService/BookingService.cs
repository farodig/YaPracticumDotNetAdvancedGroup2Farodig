using LearningWebApi.Entities;
using LearningWebApi.Entities.Factories;
using LearningWebApi.Repositories;
using System.Collections.Concurrent;

namespace LearningWebApi.Services.BookingService
{
    internal class BookingService(IEventRepository eventRepository, IBookingRepository bookingRepository) : IBookingService
    {
        private readonly IEventRepository _eventRepository = eventRepository;
        private readonly IBookingRepository _bookingRepository = bookingRepository;
        private readonly ConcurrentDictionary<Guid, Guid> _mapEventToBooking = new();

        public async ValueTask<Booking?> CreateBookingAsync(Guid eventId)
        {
            if (!_eventRepository.ContainsKey(eventId))
            {
                // Нельзя забронировать, т. к. событие не найдено
                return null;
            }

            if (_mapEventToBooking.TryGetValue(eventId, out Guid bookingId)
                && _bookingRepository.TryGetValue(bookingId, out Booking? booking)
                && booking is not null)
            {
                // Событие уже забронировано, возвращаем ту же самую бронь
                return booking;
            }

            booking = BookingFactory.CreateBooking(eventId);
            _bookingRepository.Add(booking.Id, booking);
            _mapEventToBooking[eventId] = booking.Id;
            return booking;
        }

        public async ValueTask<Booking?> GetBookingByIdAsync(Guid id)
        {
            _bookingRepository.TryGetValue(id, out Booking? item);
            return item;
        }
    }
}
