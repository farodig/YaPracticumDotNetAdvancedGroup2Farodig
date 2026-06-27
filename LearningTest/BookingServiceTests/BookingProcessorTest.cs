using static LearningTest.Factories.EntityFactory;
using static LearningTest.Factories.ServiceFactory;
using static LearningTest.Factories.MockRepositoryFactory;
using LearningWebApi.Services.BookingService;

namespace LearningTest.BookingServiceTests
{
    public class BookingProcessorTest
    {
        [Fact(DisplayName = "Проверка корректной отмены обработчика BookingProcessor")]
        public async Task Test()
        {
            var @event = CreateEventAvailableSeats();
            var eventRepository = MockEventRepository(@event);
            var eventService = CreateEventService(eventRepository);
            var bookingService = CreateBookingService(eventService);

            // Создали бронь
            var booking = bookingService.CreateBooking(@event.Id);

            using var cts = new CancellationTokenSource();
            using var bookingProcessor = new BookingProcessor(bookingService, eventRepository);

            // Начали обрабатывать бронь
            //var process =  bookingProcessor.ProcessBookingAsync(booking, cts.Token);
            var process = Task.Run(() => bookingProcessor.ProcessBookingAsync(booking, cts.Token));

            // Отменили операцию
            cts.Cancel();

            await process;

            // Убедились что созданной брони не существует
            Assert.Null(bookingService.GetBookingById(booking.Id));
        }
    }
}
