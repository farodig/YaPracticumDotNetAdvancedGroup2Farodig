using Application.Services.BookingService;
using Application.Services.EventService;
using Domain.Entities;
using Domain.Exceptions;
using UnitTests.Helpers;
using static UnitTests.Helpers.EntityFactory;

namespace UnitTests.BookingServiceTests
{
    [Trait("Category", "Unit")]
    public class BookingProcessorTest : AServiceCollection
    {
        [Fact(DisplayName = "01. Проверка корректной отмены обработчика BookingProcessor")]
        public async Task CancelBookingProcessTest()
        {
            var personId = Guid.NewGuid();
            var @event = CreateEvent(totalSeats: 1);
            var bookingService = GetInitializedService<IBookingService, Event>(@event);

            // Создали бронь
            var booking = await bookingService.CreateBookingAsync(@event.Id, personId);

            using var cts = new CancellationTokenSource();
            using var bookingProcessor = GetHostedService<BookingProcessor>()!;

            // Начали обрабатывать бронь
            var process = bookingProcessor.ProcessBookingAsync(booking.BuildBooking(), cts.Token);

            // Отменили операцию
            cts.Cancel();

            await process;

            // Убедились что созданной брони не существует

            await Assert.ThrowsAsync<BookingNotFoundException>(async () => await bookingService.GetBookingByIdAsync(booking.Id));
        }

        [Fact(DisplayName = "02. Проверка успешной обработки бронирования события")]
        public async Task ProcessSuccessBookingEventTest()
        {
            var eventId = Guid.NewGuid();
            Initialize(CreateEvent(eventId: eventId, totalSeats: 1));
            var booking = CreateBooking(eventId: eventId);
            Initialize(booking);

            using var bookingProcessor = GetHostedService<BookingProcessor>()!;
            await bookingProcessor.ProcessBookingAsync(booking, CancellationToken.None);

            Assert.Equal(BookingStatus.Confirmed, booking.Status);
        }

        [Fact(DisplayName = "03. Проверка обработки бронирования события которое было удалено")]
        public async Task ProcessBookingNotExistedEventTest()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            Initialize(CreateEvent(eventId: eventId, totalSeats: 1));
            var bookingId = Guid.NewGuid();
            Initialize(CreateBooking(bookingId: bookingId, eventId: eventId));
            var eventService = GetService<IEventService>();
            var bookingService = GetService<IBookingService>();

            // Act
            // Удалить событие
            await eventService.TryDeleteEventAsync(eventId);
            
            // Assert
            // т. к. событие удалено, то и брони удаляются каскадно, следовательно безвозвратно, а не просто меняют статус
            await Assert.ThrowsAsync<BookingNotFoundException>(async () => await bookingService.GetBookingByIdAsync(bookingId));
        }
    }
}
