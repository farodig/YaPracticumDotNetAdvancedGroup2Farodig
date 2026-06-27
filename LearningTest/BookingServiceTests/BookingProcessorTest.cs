using LearningWebApi.Services.BookingService;
using static LearningTest.Factories.EntityFactory;
using static LearningTest.Factories.MockRepositoryFactory;
using static LearningTest.Factories.ServiceFactory;
using static LearningTest.Factories.RepositoryFactory;
using LearningWebApi.Entities;

namespace LearningTest.BookingServiceTests
{
    public class BookingProcessorTest
    {
        [Fact(DisplayName = "Проверка корректной отмены обработчика BookingProcessor")]
        public async Task CancelBookingProcessTest()
        {
            var @event = CreateEventAvailableSeats();
            var eventRepository = MockEventRepository(@event);
            var eventService = CreateEventService(eventRepository);
            var bookingService = CreateBookingService(eventService);

            // Создали бронь
            var booking = bookingService.CreateBooking(@event.Id);

            using var cts = new CancellationTokenSource();
            using var bookingProcessor = new BookingProcessor(bookingService, eventService);

            // Начали обрабатывать бронь
            var process = Task.Run(() => bookingProcessor.ProcessBookingAsync(booking, cts.Token));

            // Отменили операцию
            cts.Cancel();

            await process;

            // Убедились что созданной брони не существует
            Assert.Null(bookingService.GetBookingById(booking.Id));
        }

        [Fact(DisplayName = "Проверка успешной обработки бронирования события")]
        public async Task ProcessSuccessBookingEventTest()
        {
            var @event = CreateEventAvailableSeats();
            var eventRepository = MockEventRepository(@event);
            var eventService = CreateEventService(eventRepository);
            var bookingService = CreateBookingService(eventService);

            // Создать бронь
            var booking = bookingService.CreateBooking(@event.Id);

            using var bookingProcessor = new BookingProcessor(bookingService, eventService);
            await bookingProcessor.ProcessBookingAsync(booking, CancellationToken.None);

            Assert.Equal(BookingStatus.Confirmed, booking.Status);
        }

        [Fact(DisplayName = "Проверка обработки бронирования события которое было удалено")]
        public async Task ProcessBookingNotExistedEventTest()
        {
            var @event = CreateEventAvailableSeats();
            var eventRepository = CreateEventRepository(@event);
            var eventService = CreateEventService(eventRepository);
            var bookingService = CreateBookingService(eventService);

            // Создать бронь
            var booking = bookingService.CreateBooking(@event.Id);
            // Удалить бронь
            eventService.TryDeleteEvent(@event.Id);

            using var bookingProcessor = new BookingProcessor(bookingService, eventService);
            await bookingProcessor.ProcessBookingAsync(booking, CancellationToken.None);

            Assert.Equal(BookingStatus.Rejected, booking.Status);
        }
    }
}
