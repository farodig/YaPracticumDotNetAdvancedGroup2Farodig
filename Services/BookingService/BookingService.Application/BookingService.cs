using BookingService.Application.Abstractions;
using BookingService.Application.Models.Builders;
using BookingService.Application.Models.Responses;
using BookingService.Domain.Entities;
using BookingService.Domain.Exceptions;
using BrokerService.Application;
using NLog;
using SharedContracts.Events;
using System.Data;
using TokenService.Exceptions;

namespace BookingService.Application
{
    public class BookingService(IBookingRepository repository, IPublishService publishService) : IBookingService
    {
        private readonly IBookingRepository _repository = repository;
        private readonly IPublishService _publishService = publishService;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
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

                await _publishService.PublishAsync(booking.ToBookingCreatedEvent(), cts);
                _logger.Info($"Booking #{booking.Id} created with status '{booking.Status}'");
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
            _logger.Info($"Booking operation was confirmed. Event Id = '{data.EventId}', Booking Id = '{data.Id}'");
        }

        public async Task RejectBookingAsync(Booking data, CancellationToken cts = default)
        {
            await _repository.TryUpdateStatusAsync(data, BookingStatus.Rejected, cts);
            _logger.Warn($"Booking operation was rejected'. Event Id = '{data.EventId}', Booking Id = '{data.Id}'");
        }

        public async Task CancelBookingByAdminAsync(Guid bookingId, CancellationToken cts = default)
        {
            var booking = await _repository.GetAsync(bookingId, cts) ?? throw new BookingNotFoundException();

            if (booking.Status != BookingStatus.Confirmed) throw new InvalidOperationException("Unable to cancel not confirmed booking");

            await _repository.TryUpdateStatusAsync(booking, BookingStatus.Cancelled, cts);

            await _publishService.PublishAsync(booking.ToBookingCancelEvent(CancelReasonType.CancelByAdmin), cts);
            _logger.Warn($"Booking operation was cancelled by the Admin. Event Id = '{booking.EventId}', Booking Id = '{booking.Id}'");
        }

        public async Task CancelBookingByPersonAsync(Guid bookingId, Guid personId, CancellationToken cts = default)
        {
            var booking = await _repository.GetAsync(bookingId, cts) ?? throw new BookingNotFoundException();

            if (booking.PersonId != personId) throw new UnauthorizedBookingOperationException();

            if (booking.Status != BookingStatus.Confirmed) throw new InvalidOperationException("Unable to cancel not confirmed booking");

            await _repository.TryUpdateStatusAsync(booking, BookingStatus.Cancelled, cts);
            await _publishService.PublishAsync(booking.ToBookingCancelEvent(CancelReasonType.CancelByPerson), cts);
            _logger.Info($"Booking operation was cancelled by the person '{personId}'. Event Id = '{booking.EventId}', Booking Id = '{booking.Id}'");
        }
    }
}
