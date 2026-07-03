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
        private readonly IBookingRepository _repository = bookingRepository;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly SemaphoreSlim _bookingSemaphore = new(initialCount: 1, maxCount: 1);

        public async Task<Booking> CreateBookingAsync(Guid eventId, CancellationToken? cts = null)
        {
            await _bookingSemaphore.WaitAsync(cts ?? CancellationToken.None);
            try
            {
                await _eventService.ReserveSeatAsync(eventId, cts ?? CancellationToken.None);

                // Создать бронь
                var booking = BookingFactory.CreateBooking(eventId);
                _repository.CreateOrUpdate(booking);
                _logger.Info($"Booking #{booking.Id} created with status '{booking.Status}'");
                return booking;
            }
            finally
            {
                _bookingSemaphore.Release();
            }
        }

        public Booking? GetBookingById(Guid id)
        {
            return _repository.Get(id);
        }

        public void CancelBooking(Guid bookingId)
        {
            _repository.Remove(bookingId);
        }

        public IEnumerable<Booking> GetPending()
        {
            return _repository.Select(a => a.Value)
                .Where(a => a.Status == BookingStatus.Pending)
                .OrderBy(a => a.CreatedAt);
        }

        public void ConfirmBooking(Booking data)
        {
            data.Status = BookingStatus.Confirmed;
            data.ProcessedAt = DateTime.Now;
            _logger.Info($"Booking #{data.Id} changed status to '{data.Status}'");
            _repository.CreateOrUpdate(data);
        }

        public async Task RejectBookingAsync(Booking data, CancellationToken? cts = null)
        {
            data.Status = BookingStatus.Rejected;
            data.ProcessedAt = DateTime.Now;
            _logger.Warn($"Booking #{data.Id} changed status to '{data.Status}'");
            _repository.CreateOrUpdate(data);

            await _eventService.ReleaseSeatAsync(data.EventId, cts ?? CancellationToken.None);
        }
    }
}
