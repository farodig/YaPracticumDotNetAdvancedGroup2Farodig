using Application.Models.Builders;
using Application.Models.Responses;
using Application.Repositories;
using Application.Services.EventService;
using Domain.Entities;
using NLog;

namespace Application.Services.BookingService
{
    public class BookingService(IEventService eventService, IBookingRepository bookingRepository) : IBookingService
    {
        private readonly IEventService _eventService = eventService;
        private readonly IBookingRepository _repository = bookingRepository;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly SemaphoreSlim _bookingSemaphore = new(initialCount: 1, maxCount: 1);

        public async Task<BookingResponse> CreateBookingAsync(Guid eventId, Guid personId, CancellationToken cts = default)
        {
            await _bookingSemaphore.WaitAsync(cts);
            try
            {
                await _eventService.ReserveSeatAsync(eventId, cts);

                // Создать бронь
                var booking = BookingBuilder.CreateBooking(eventId, personId);
                await _repository.CreateAsync(booking, cts);
                _logger.Info($"Booking #{booking.Id} created with status '{booking.Status}'");
                return booking.BuildBookingResponse();
            }
            finally
            {
                _bookingSemaphore.Release();
            }
        }

        public async Task<BookingResponse?> GetBookingByIdAsync(Guid id, CancellationToken cts = default)
        {
            var item = await _repository.GetAsync(id, cts);
            return item?.BuildBookingResponse();
        }

        public async Task CancelBookingAsync(Booking data, CancellationToken cts = default)
        {
            await _repository.TryRemoveAsync(data.Id, cts);
            _logger.Warn($"Booking operation was cancelled. Event Id = '{data.EventId}', Booking Id = '{data.Id}'");
        }

        public async Task<IEnumerable<Booking>> GetPendingByCreatedAsync(CancellationToken cts = default)
        {
            return (await _repository.GetBookingsByStatus(Domain.Entities.BookingStatus.Pending, cts))
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
