using LearningWebApi.Entities;
using LearningWebApi.Entities.Factories;
using LearningWebApi.Repositories;
using LearningWebApi.Services.EventService;
using NLog;

namespace LearningWebApi.Services.BookingService
{
    internal class BookingService(IEventService eventService, IBookingRepository bookingRepository) : IBookingService
    {
        private readonly IEventService _eventService = eventService;
        private readonly IBookingRepository _bookingRepository = bookingRepository;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly Lock _bookingLock = new();

        public Booking CreateBooking(Guid eventId)
        {
            lock (_bookingLock)
            {
                // TODO: по хорошему следует вынести за пределы lock (_bookingLock), но в ТЗ говорит резервировать места нужно с двойной блокировкой
                _eventService.ReserveSeat(eventId);

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

        public IEnumerable<Booking> GetPending()
        {
            return _bookingRepository.Select(a => a.Value)
                .Where(a => a.Status == BookingStatus.Pending)
                .OrderBy(a => a.CreatedAt);
        }

        public void ConfirmBooking(Booking data)
        {
            data.Status = BookingStatus.Confirmed;
            data.ProcessedAt = DateTime.Now;
            _logger.Info($"Booking #{data.Id} changed status to '{data.Status}'");
            _bookingRepository[data.Id] = data;
        }

        public void RejectBooking(Booking data)
        {
            data.Status = BookingStatus.Rejected;
            data.ProcessedAt = DateTime.Now;
            _logger.Warn($"Booking #{data.Id} changed status to '{data.Status}'");
            _bookingRepository[data.Id] = data;

            _eventService.ReleaseSeat(data.EventId);
        }
    }
}
