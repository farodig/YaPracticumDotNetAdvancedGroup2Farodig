using LearningWebApi.Entities;
using LearningWebApi.Services.BookingService;
using static LearningTest.Factories.EntityFactory;
using static LearningTest.Factories.ServiceFactory;
using static LearningTest.Factories.MockRepositoryFactory;
using static LearningTest.Factories.RepositoryFactory;

namespace LearningTest.BookingServiceTests
{
    public class BookingProcessorTest
    {
        [Fact(DisplayName = "После вызова Confirm() бронь возвращает статус Confirmed и заполненный ProcessedAt")]
        public async Task ChangeBookingStatusFromPendingToConfirmedTest()
        {
            var @event = CreateEventAvailableSeats();

            var eventRepository = MockEventRepository(@event);
            var bookingRepository = CreateBookingRepository();
            var bookingService = CreateBookingService(bookingRepository, eventRepository);
            using var bookingProcessor = new BookingProcessor(bookingRepository, eventRepository);
            await bookingProcessor.StartAsync(CancellationToken.None);

            var booking = bookingService.CreateBooking(@event.Id);
            Assert.Equal(BookingStatus.Pending, booking.Status);

            booking = bookingService.GetBookingById(booking.Id);
            Assert.NotNull(booking);
            Assert.Equal(BookingStatus.Pending, booking.Status);

            await Task.Delay(TimeSpan.FromSeconds(5));

            booking = bookingService.GetBookingById(booking.Id);
            Assert.NotNull(booking);
            Assert.NotNull(booking.ProcessedAt);
            Assert.Equal(BookingStatus.Confirmed, booking.Status);
        }

        [Fact(DisplayName = "После вызова Reject() бронь возвращает статус Rejected и заполненный ProcessedAt")]
        public async Task RejectBookingTest()
        {
            var @event = CreateEventAvailableSeats();
            var eventRepository = CreateEventRepository(@event);
            var bookingRepository = CreateBookingRepository();
            var bookingService = CreateBookingService(bookingRepository, eventRepository);
            using var bookingProcessor = new BookingProcessor(bookingRepository, eventRepository);
            await bookingProcessor.StartAsync(CancellationToken.None);

            var booking = bookingService.CreateBooking(@event.Id);
            Assert.Equal(BookingStatus.Pending, booking.Status);
            // Удалить событие чтобы вызвать Reject
            eventRepository.Remove(@event.Id);

            await Task.Delay(TimeSpan.FromSeconds(5));

            booking = bookingService.GetBookingById(booking.Id);
            Assert.NotNull(booking);
            Assert.NotNull(booking.ProcessedAt);
            Assert.Equal(BookingStatus.Rejected, booking.Status);
        }

        [Fact(DisplayName = "После Reject()\r\nReleaseSeats() количество свободных мест восстанавливается")]
        public async Task RejectAvailableSeatsTest()
        {
            // TODO: ждём ответа от куратора

            //var @event = CreateEventAvailableSeats();

            //var eventRepository = MockEventRepository(@event);
            //var bookingRepository = CreateBookingRepository();
            //var bookingService = CreateBookingService(bookingRepository, eventRepository);
            //using var bookingProcessor = new BookingProcessor(bookingRepository, eventRepository);
            //await bookingProcessor.StartAsync(CancellationToken.None);

            //var booking = bookingService.CreateBooking(@event.Id);

            //await Task.Delay(TimeSpan.FromSeconds(3));

            //booking = bookingService.GetBookingById(booking.Id);
            //Assert.NotNull(booking);
            //Assert.NotNull(booking.ProcessedAt);
            //Assert.Equal(BookingStatus.Confirmed, booking.Status);
        }
        [Fact(DisplayName = "После Reject()\r\nReleaseSeats() можно успешно создать новую бронь на то же место")]
        public async Task RejectAndCreateBookingTest()
        {
            // TODO: ждём ответа от куратора
        }
    }
}
