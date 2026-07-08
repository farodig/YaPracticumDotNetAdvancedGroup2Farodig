using Learning.UnitTests.Helpers;
using LearningWebApi.Entities;
using LearningWebApi.Services.BookingService;
using LearningWebApi.Services.EventService;
using static Learning.UnitTests.Helpers.EntityFactory;

namespace Learning.UnitTests.BookingServiceTests
{
    [Trait("Category", "Unit")]
    public class BookingProcessorTest : AServiceCollection
    {
        [Fact(DisplayName = "01. Проверка корректной отмены обработчика BookingProcessor")]
        public async Task CancelBookingProcessTest()
        {
            var @event = CreateEventAvailableSeats();
            var bookingService = GetInitializedService<IBookingService, Event>(@event);

            // Создали бронь
            var booking = await bookingService.CreateBookingAsync(@event.Id);

            using var cts = new CancellationTokenSource();
            using var bookingProcessor = GetHostedService<BookingProcessor>()!;

            // Начали обрабатывать бронь
            var process = bookingProcessor.ProcessBookingAsync(booking, cts.Token);

            // Отменили операцию
            cts.Cancel();

            await process;

            // Убедились что созданной брони не существует
            Assert.Null(await bookingService.GetBookingByIdAsync(booking.Id));
        }

        [Fact(DisplayName = "02. Проверка успешной обработки бронирования события")]
        public async Task ProcessSuccessBookingEventTest()
        {
            var @event = CreateEventAvailableSeats();
            var bookingService = GetInitializedService<IBookingService, Event>(@event);

            // Создать бронь
            var booking = await bookingService.CreateBookingAsync(@event.Id);

            using var bookingProcessor = GetHostedService<BookingProcessor>()!;
            await bookingProcessor.ProcessBookingAsync(booking, CancellationToken.None);

            Assert.Equal(BookingStatus.Confirmed, booking.Status);
        }

        [Fact(DisplayName = "03. Проверка обработки бронирования события которое было удалено")]
        public async Task ProcessBookingNotExistedEventTest()
        {
            var @event = CreateEventAvailableSeats();
            var (eventService, bookingService) = GetInitializedServices<IEventService, IBookingService, Event>(@event);

            // Создать бронь
            var booking = await bookingService.CreateBookingAsync(@event.Id);
            // Удалить бронь
            await eventService.TryDeleteEventAsync(@event.Id);

            using var bookingProcessor = GetHostedService<BookingProcessor>()!;
            await bookingProcessor.ProcessBookingAsync(booking, CancellationToken.None);

            Assert.Equal(BookingStatus.Rejected, booking.Status);
        }
    }
}
