using BookingService.Application.Abstractions;
using BookingService.Application.Models.Builders;
using BookingService.Application.Models.Responses;
using BookingService.Domain.Entities;
using BookingService.Domain.Exceptions;
using NLog;
using PublishService.Application;
using System.Data;
using TokenService.Exceptions;

namespace BookingService.Application
{
    public class BookingService(/*IReservationService reservationService, */
        IPublishService publishService,
        IBookingRepository bookingRepository) : IBookingService
    {
        private readonly IPublishService _publishService = publishService;

        //private readonly IReservationService _reservationService = reservationService;
        private readonly IBookingRepository _repository = bookingRepository;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly SemaphoreSlim _bookingSemaphore = new(initialCount: 1, maxCount: 1);

        public async Task<BookingResponse> CreateBookingAsync(Guid eventId, Guid personId, CancellationToken cts = default)
        {
            await _bookingSemaphore.WaitAsync(cts);
            try
            {
                //await _reservationService.ReserveSeatAsync(eventId, personId, cts);

                // Создать бронь
                var booking = BookingBuilder.CreateBooking(eventId, personId);
                await _repository.CreateAsync(booking, cts);
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
            await _publishService.PublishAsync(data.ToSuccessEvent(), cts);
            _logger.Info($"Booking operation was confirmed. Event Id = '{data.EventId}', Booking Id = '{data.Id}'");
        }

        public async Task RejectBookingAsync(Booking data, CancellationToken cts = default)
        {
            //await _reservationService.ReleaseSeatAsync(data, BookingStatus.Rejected, cts);
            await _publishService.PublishAsync(data.ToFailureEvent(), cts);
            _logger.Info($"Booking operation was rejected'. Event Id = '{data.EventId}', Booking Id = '{data.Id}'");
        }

        public async Task CancelBookingAsync(Booking data, CancellationToken cts = default)
        {
            //await _reservationService.ReleaseSeatAsync(data, BookingStatus.Cancelled, cts);
            await _publishService.PublishAsync(data.ToFailureEvent(), cts);
            _logger.Warn($"Booking operation was cancelled. Event Id = '{data.EventId}', Booking Id = '{data.Id}'");
        }

        public async Task CancelBookingByAdminAsync(Guid bookingId, CancellationToken cts = default)
        {
            var booking = await _repository.GetAsync(bookingId, cts) ?? throw new BookingNotFoundException();

            //await _reservationService.ReleaseSeatAsync(booking, BookingStatus.Cancelled, cts);
            await _publishService.PublishAsync(booking.ToFailureEvent(), cts);
            _logger.Warn($"Booking operation was cancelled by the Admin. Event Id = '{booking.EventId}', Booking Id = '{booking.Id}'");
        }

        public async Task CancelBookingByPersonAsync(Guid bookingId, Guid personId, CancellationToken cts = default)
        {
            var booking = await _repository.GetAsync(bookingId, cts) ?? throw new BookingNotFoundException();

            if (booking.PersonId != personId) throw new UnauthorizedBookingOperationException();

            //await _reservationService.ReleaseSeatAsync(booking, BookingStatus.Cancelled, cts);
            await _publishService.PublishAsync(booking.ToFailureEvent(), cts);
            _logger.Warn($"Booking operation was cancelled by the person '{personId}'. Event Id = '{booking.EventId}', Booking Id = '{booking.Id}'");
        }
    }
}
