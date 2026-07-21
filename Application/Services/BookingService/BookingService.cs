using Application.Abstractions;
using Application.Models.Builders;
using Application.Models.Responses;
using Application.Services.ReservationService;
using Domain.Entities;
using Domain.Exceptions;
using NLog;

namespace Application.Services.BookingService
{
    public class BookingService(IReservationService reservationService, IBookingRepository bookingRepository) : IBookingService
    {
        private readonly IReservationService _reservationService = reservationService;
        private readonly IBookingRepository _repository = bookingRepository;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly SemaphoreSlim _bookingSemaphore = new(initialCount: 1, maxCount: 1);

        public async Task<BookingResponse> CreateBookingAsync(Guid eventId, Guid personId, CancellationToken cts = default)
        {
            await _bookingSemaphore.WaitAsync(cts);
            try
            {
                await _reservationService.ReserveSeatAsync(eventId, personId, cts);

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

        public async Task<BookingResponse> GetBookingByIdAsync(Guid id, CancellationToken cts = default)
        {
            var item = await _repository.GetAsync(id, cts) ?? throw new BookingNotFoundException();
            return item.BuildBookingResponse();
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
            await _reservationService.ReleaseSeatAsync(data, BookingStatus.Rejected, cts);
            _logger.Info($"Booking operation was rejected'. Event Id = '{data.EventId}', Booking Id = '{data.Id}'");
        }

        public async Task CancelBookingAsync(Booking data, CancellationToken cts = default)
        {
            await _reservationService.ReleaseSeatAsync(data, BookingStatus.Cancelled, cts);
            _logger.Warn($"Booking operation was cancelled. Event Id = '{data.EventId}', Booking Id = '{data.Id}'");
        }

        public async Task CancelBookingAsync(Guid bookingId, Guid personId, PersonRole role, CancellationToken cts = default)
        {
            var booking = await _repository.GetWithPersonAsync(bookingId, cts) ?? throw new BookingNotFoundException();

            if (role != PersonRole.Admin && booking.Person?.Id != personId) throw new UnauthorizedBookingOperationException();

            await _reservationService.ReleaseSeatAsync(booking, BookingStatus.Cancelled, cts);
            _logger.Warn($"Booking operation was cancelled by the '{role}'. Event Id = '{booking.EventId}', Booking Id = '{booking.Id}'");
        }
    }
}
