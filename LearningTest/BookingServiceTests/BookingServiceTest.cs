using LearningWebApi.Entities;
using LearningWebApi.Exceptions;
using static LearningTest.Factories.EntityFactory;
using static LearningTest.Factories.ServiceFactory;
using static LearningTest.Factories.MockRepositoryFactory;
using static LearningTest.Factories.RepositoryFactory;

namespace LearningTest.BookingServiceTests
{
    public class BookingServiceTest
    {
        [Fact(DisplayName = "создание брони для существующего события — возвращается BookingInfo со статусом Pending")]
        public async Task CreateBookingTest()
        {
            var @event = CreateEventAvailableSeats();
            var eventRepository = MockEventRepository(@event);
            var booking = CreateBookingService(eventRepository)
                .CreateBooking(@event.Id);

            Assert.Equal(@event.Id, booking.EventId);
            Assert.Equal(BookingStatus.Pending, booking.Status);
        }

        [Fact(DisplayName = "Создание нескольких броней (до лимита) — все успешны, у каждой уникальный Id")]
        public async Task CreateFewBookingsTest()
        {
            var @event = CreateEventAvailableSeats(2);
            var eventRepository = MockEventRepository(@event);

            var bookingService = CreateBookingService(eventRepository);
            var booking1 = bookingService.CreateBooking(@event.Id);
            var booking2 = bookingService.CreateBooking(@event.Id);

            Assert.Equal(@event.Id, booking1.EventId);
            Assert.Equal(@event.Id, booking2.EventId);

            Assert.NotEqual(booking1.Id, booking2.Id);
        }

        [Fact(DisplayName = "получение брони по Id — возвращается корректная информация")]
        public async Task GetBookingByIdAsyncTest()
        {
            var expectedBooking = CreateBooking();

            var bookingRepository = MockBookingRepository(expectedBooking);
            var bookingService = CreateBookingService(bookingRepository);

            var actualBooking = bookingService.GetBookingById(expectedBooking.Id);

            Assert.NotNull(actualBooking);
            Assert.Equal(expectedBooking.Id, actualBooking.Id);
        }

        [Fact(DisplayName = "Бронирование для несуществующего события → NotFoundException")]
        public async Task CreateBookingNotFoundExceptionTest()
        {
            var bookingService = CreateBookingService();
            Assert.Throws<EventNotFoundException>(() => bookingService.CreateBooking(Guid.NewGuid()));
        }

        [Fact(DisplayName = "создание брони для удалённого события")]
        public async Task CreateBookingForDeletedEventTest()
        {
            var @event = CreateEvent();
            var eventRepository = CreateEventRepository(@event);

            var bookingService = CreateBookingService(eventRepository);

            Assert.True(eventRepository.TryRemove(@event.Id, out _));
            Assert.False(eventRepository.ContainsKey(@event.Id));

            Assert.Throws<EventNotFoundException>(() => bookingService.CreateBooking(@event.Id));
        }

        [Fact(DisplayName = "получение брони по несуществующему Id")]
        public async Task GetNotExitedBookingTest()
        {
            var bookingService = CreateBookingService();
            var booking = bookingService.GetBookingById(Guid.NewGuid());
            Assert.Null(booking);
        }

        [Fact(DisplayName = "Создание брони уменьшает AvailableSeats на 1")]
        public async Task CreateBookingDecreaseAvailableSeatsByOneTest()
        {
            var expectedAvailableSeats = 2;

            var @event = CreateEventAvailableSeats(expectedAvailableSeats + 1);
            var eventRepository = MockEventRepository(@event);

            CreateBookingService(eventRepository)
                .CreateBooking(@event.Id);

            Assert.Equal(expectedAvailableSeats, @event.AvailableSeats);
        }

        [Fact(DisplayName = "Бронирование при отсутствии|исчерпании мест → NoAvailableSeatsException")]
        public async Task CreateBookingNoAvailableSeatsExceptionTest()
        {
            var @event = CreateEventAvailableSeats(0);
            var eventRepository = MockEventRepository(@event);
            var bookingService = CreateBookingService(eventRepository);

            Assert.Throws<NoAvailableSeatsException>(() => bookingService.CreateBooking(@event.Id));
        }
    }

}
