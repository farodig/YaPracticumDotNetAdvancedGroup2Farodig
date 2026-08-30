using BookingService.Application.Abstractions;
using BookingService.Application.Models.Builders;
using BookingService.Application.Models.Responses;
using BookingService.Domain.Entities;
using BookingService.Domain.Exceptions;
using BrokerService.Application;
using Microsoft.Extensions.Logging;
using SharedContracts.Events.BookingEvents;
using System.Data;
using TokenService.Exceptions;

namespace BookingService.Application
{
    public class BookingService(IBookingRepository repository, IPublishService publishService, ILogger<BookingService> logger) : IBookingService
    {
        private readonly IBookingRepository _repository = repository;
        private readonly IPublishService _publishService = publishService;
        private readonly ILogger<BookingService> _logger = logger;
        private readonly SemaphoreSlim _bookingSemaphore = new(initialCount: 1, maxCount: 1);

        public async Task<BookingResponse> CreateBookingAsync(Guid eventId, Guid personId, CancellationToken cts = default)
        {
            await _bookingSemaphore.WaitAsync(cts);
            try
            {
                // Пользователь достиг лимита на количество активных броней
                if (IBookingService.PersonMaxBookingCount <= await _repository.GetBookingCountAsync(personId, cts))
                    throw new ActiveBookingLimitException(limit: IBookingService.PersonMaxBookingCount);

                var booking = BookingBuilder.CreateBooking(eventId, personId);
                await _repository.CreateAsync(booking, cts);

                await _publishService.PublishBookingCreatedEvent(booking, cts);
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Booking {Id} created with {Status}", booking.Id, booking.Status);
                }
                return booking.ToResponse();
            }
            finally
            {
                _bookingSemaphore.Release();
            }
        }

        public async Task<BookingResponse> GetBookingByIdAsync(Guid id, CancellationToken cts = default)
        {
            var item = await _repository.GetAsync(id, cts) ?? throw new BookingNotFoundException();
            return item.ToResponse();
        }

        public async Task<IEnumerable<Booking>> GetPendingByCreatedAsync(CancellationToken cts = default)
        {
            return (await _repository.GetBookingsByStatus(BookingStatus.Pending, cts))
                .OrderBy(a => a.CreatedAt);
        }

        public async Task ConfirmBookingAsync(Booking data, CancellationToken cts = default)
        {
            await _repository.TryUpdateStatusAsync(data, BookingStatus.Confirmed, cts);
            await _publishService.PublishBookingConfirmedEvent(data, cts);
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Booking operation was confirmed {EventId}, {BookingId}", data.EventId, data.Id);
            }
        }

        public async Task RejectBookingAsync(Booking data, CancellationToken cts = default)
        {
            await _repository.TryUpdateStatusAsync(data, BookingStatus.Rejected, cts);
            await _publishService.PublishBookingRejectedEvent(data, cts);
            _logger.LogWarning("Booking operation was rejected {EventId}, {BookingId}", data.EventId, data.Id);
        }

        public async Task CancelBookingByAdminAsync(Guid bookingId, CancellationToken cts = default)
        {
            var booking = await _repository.GetAsync(bookingId, cts) ?? throw new BookingNotFoundException();

            if (booking.Status != BookingStatus.Confirmed) throw new InvalidOperationException("Unable to cancel not confirmed booking");

            await _repository.TryUpdateStatusAsync(booking, BookingStatus.Cancelled, cts);

            await _publishService.PublishBookingCancelEvent(booking, CancelReasonType.CancelByAdmin, cts);
            _logger.LogWarning("Booking operation was cancelled by the Admin {EventId}, {BookingId}", booking.EventId, booking.Id);
        }

        public async Task CancelBookingByPersonAsync(Guid bookingId, Guid personId, CancellationToken cts = default)
        {
            var booking = await _repository.GetAsync(bookingId, cts) ?? throw new BookingNotFoundException();

            if (booking.PersonId != personId) throw new UnauthorizedBookingOperationException();

            if (booking.Status != BookingStatus.Confirmed) throw new InvalidOperationException("Unable to cancel not confirmed booking");

            await _repository.TryUpdateStatusAsync(booking, BookingStatus.Cancelled, cts);

            await _publishService.PublishBookingCancelEvent(booking, CancelReasonType.CancelByPerson, cts);
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Booking operation was cancelled by the {personId}, {EventId}, {BookingId}", personId, booking.EventId, booking.Id);
            }
        }
    }
}
