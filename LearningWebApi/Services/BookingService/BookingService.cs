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

        public async Task<Booking> CreateBookingAsync(Guid eventId, CancellationToken cts = default)
        {
            await _bookingSemaphore.WaitAsync(cts);
            try
            {
                await _eventService.ReserveSeatAsync(eventId, cts);

                // Создать бронь
                var booking = BookingFactory.CreateBooking(eventId);
                await _repository.CreateAsync(booking, cts);
                _logger.Info($"Booking #{booking.Id} created with status '{booking.Status}'");
                return booking;
            }
            finally
            {
                _bookingSemaphore.Release();
            }
        }

        public async Task<Booking?> GetBookingByIdAsync(Guid id, CancellationToken cts = default)
        {
            return await _repository.GetAsync(id, cts);
        }

        public async Task CancelBookingAsync(Booking data, CancellationToken cts = default)
        {
            await _repository.TryRemoveAsync(data.Id, cts);
            _logger.Warn($"Booking operation was cancelled. Event Id = '{data.EventId}', Booking Id = '{data.Id}'");
        }

        public async Task<IEnumerable<Booking>> GetPendingByCreatedAsync(CancellationToken cts = default)
        {
            return (await _repository.GetBookingsByStatus(BookingStatus.Pending))
                .OrderBy(a => a.CreatedAt);
        }

        public async Task ConfirmBookingAsync(Booking data, CancellationToken cts = default)
        {
            data.Status = BookingStatus.Confirmed;
            data.ProcessedAt = DateTime.Now;
            await _repository.TryUpdateAsync(data, cts);
            _logger.Info($"Booking #{data.Id} changed status to '{data.Status}'");
        }

        public async Task RejectBookingAsync(Booking data, CancellationToken cts = default)
        {
            data.Status = BookingStatus.Rejected;
            data.ProcessedAt = DateTime.Now;
            await _eventService.ReleaseSeatAsync(data.EventId, cts);
            await _repository.TryUpdateAsync(data, cts);
            _logger.Warn($"Booking #{data.Id} changed status to '{data.Status}'");
        }
    }
}
