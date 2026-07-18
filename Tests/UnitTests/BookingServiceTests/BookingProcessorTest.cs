using Application.Services.BookingService;
using Application.Services.EventService;
using Domain.Entities;
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
            Assert.Null(await bookingService.GetBookingByIdAsync(booking.Id));
        }

        [Fact(DisplayName = "02. Проверка успешной обработки бронирования события")]
        public async Task ProcessSuccessBookingEventTest()
        {
            var personId = Guid.NewGuid();
            var @event = CreateEvent(totalSeats: 1);
            var bookingService = GetInitializedService<IBookingService, Event>(@event);

            // Создать бронь
            var booking = (await bookingService.CreateBookingAsync(@event.Id, personId))
                .BuildBooking();

            using var bookingProcessor = GetHostedService<BookingProcessor>()!;
            await bookingProcessor.ProcessBookingAsync(booking, CancellationToken.None);

            Assert.Equal(BookingStatus.Confirmed, booking.Status);
        }

        [Fact(DisplayName = "03. Проверка обработки бронирования события которое было удалено")]
        public async Task ProcessBookingNotExistedEventTest()
        {
            var personId = Guid.NewGuid();
            var @event = CreateEvent(totalSeats: 1);
            var (eventService, bookingService) = GetInitializedServices<IEventService, IBookingService, Event>(@event);

            // Создать бронь
            var booking = (await bookingService.CreateBookingAsync(@event.Id, personId))
                .BuildBooking();
            // Удалить бронь
            await eventService.TryDeleteEventAsync(@event.Id);

            using var bookingProcessor = GetHostedService<BookingProcessor>()!;
            await bookingProcessor.ProcessBookingAsync(booking, CancellationToken.None);

            Assert.Equal(BookingStatus.Rejected, booking.Status);
        }
    }
}
