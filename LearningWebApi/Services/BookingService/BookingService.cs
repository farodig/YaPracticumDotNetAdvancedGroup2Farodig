using LearningWebApi.Entities;
using LearningWebApi.Entities.Factories;
using LearningWebApi.Exceptions;
using LearningWebApi.Repositories;
using NLog;

namespace LearningWebApi.Services.BookingService
{
    internal class BookingService(IEventRepository eventRepository, IBookingRepository bookingRepository) : IBookingService
    {
        private readonly IEventRepository _eventRepository = eventRepository;
        private readonly IBookingRepository _bookingRepository = bookingRepository;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly Lock _bookingLock = new();

        public Booking? CreateBooking(Guid eventId)
        {
            lock (_bookingLock)
            {
                // Получить событие из хранилища
                if (!_eventRepository.TryGetValue(eventId, out Event? @event) || @event is null) return null;

                // Проверить на наличие свободных незарегистрированных мест
                if (!@event.TryReserveSeats()) throw new NoAvailableSeatsException();

                // Обновляем событие в репозитории TODO: сейчас в этом нет смысла, т. к. хранится в памяти, а в будущем контракт метода изменится)
                _eventRepository.TryUpdate(eventId, @event, @event);

                // Создать бронь
                var booking = BookingFactory.CreateBooking(eventId);
                _bookingRepository.Add(booking.Id, booking);
                _logger.Info($"Booking #{booking.Id} created with status '{booking.Status}'");
                return booking;
            }
        }

        public Booking? GetBookingById(Guid id)
        {
            _bookingRepository.TryGetValue(id, out Booking? item);
            return item;
        }
    }
}
