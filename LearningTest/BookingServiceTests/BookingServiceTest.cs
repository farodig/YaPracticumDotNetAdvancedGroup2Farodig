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
            var eventService = CreateEventService(eventRepository);
            var booking = CreateBookingService(eventService)
                .CreateBooking(@event.Id);

            Assert.Equal(@event.Id, booking.EventId);
            Assert.Equal(BookingStatus.Pending, booking.Status);
        }

        [Fact(DisplayName = "Создание нескольких броней (до лимита) — все успешны, у каждой уникальный Id")]
        public async Task CreateFewBookingsTest()
        {
            var @event = CreateEventAvailableSeats(2);
            var eventRepository = MockEventRepository(@event);
            var eventService = CreateEventService(eventRepository);

            var bookingService = CreateBookingService(eventService);
            var booking1 = bookingService.CreateBooking(@event.Id);
            var booking2 = bookingService.CreateBooking(@event.Id);

            Assert.Equal(@event.Id, booking1.EventId);
            Assert.Equal(@event.Id, booking2.EventId);

            Assert.NotEqual(booking1.Id, booking2.Id);
        }

        [Fact(DisplayName = "Получение брони по Id — возвращается корректная информация")]
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

        [Fact(DisplayName = "Создание брони для удалённого события")]
        public async Task CreateBookingForDeletedEventTest()
        {
            var @event = CreateEvent();
            var eventRepository = CreateEventRepository(@event);
            var eventService = CreateEventService(eventRepository);

            var bookingService = CreateBookingService(eventService);

            Assert.True(eventService.TryDeleteEvent(@event.Id));
            Assert.False(eventRepository.ContainsKey(@event.Id));

            Assert.Throws<EventNotFoundException>(() => bookingService.CreateBooking(@event.Id));
        }

        [Fact(DisplayName = "Получение брони по несуществующему Id")]
        public async Task GetNotExitedBookingTest()
        {
            var bookingService = CreateBookingService();
            var booking = bookingService.GetBookingById(Guid.NewGuid());
            Assert.Null(booking);
        }

        [Fact(DisplayName = "Создание брони уменьшает AvailableSeats на 1")]
        public async Task CreateBookingDecreaseAvailableSeatsByOneTest()
        {
            var initialSeats = 2;

            var @event = CreateEventAvailableSeats(initialSeats);
            var eventRepository = MockEventRepository(@event);
            var eventService = CreateEventService(eventRepository);
            var bookingService = CreateBookingService(eventService);

            // Бронируем пока есть места
            for (int expected = initialSeats; expected > 0;)
            {
                bookingService.CreateBooking(@event.Id);
                Assert.Equal(--expected, @event.AvailableSeats);
            }
        }

        [Fact(DisplayName = "Бронирование при отсутствии|исчерпании мест → NoAvailableSeatsException")]
        public async Task CreateBookingNoAvailableSeatsExceptionTest()
        {
            var @event = CreateEventAvailableSeats(0);
            var eventRepository = MockEventRepository(@event);
            var eventService = CreateEventService(eventRepository);
            var bookingService = CreateBookingService(eventService);

            Assert.Throws<NoAvailableSeatsException>(() => bookingService.CreateBooking(@event.Id));
        }

        [Fact(DisplayName = "После подтверждения бронь возвращает статус Confirmed и заполненный ProcessedAt")]
        public async Task BookingServiceConfirmBookingTest()
        {
            var @event = CreateEventAvailableSeats();
            var eventRepository = MockEventRepository(@event);
            var eventService = CreateEventService(eventRepository);
            var bookingService = CreateBookingService(eventService);

            var booking = bookingService.CreateBooking(@event.Id);
            bookingService.ConfirmBooking(booking);

            Assert.NotNull(booking.ProcessedAt);
            Assert.Equal(BookingStatus.Confirmed, booking.Status);
        }

        [Fact(DisplayName = "После отклонения бронь возвращает статус Rejected и заполненный ProcessedAt")]
        public async Task BookingServiceRejectBookingTest()
        {
            var @event = CreateEventAvailableSeats();
            var eventRepository = MockEventRepository(@event);
            var eventService = CreateEventService(eventRepository);
            var bookingService = CreateBookingService(eventService);

            var booking = bookingService.CreateBooking(@event.Id);
            bookingService.RejectBooking(booking);

            Assert.NotNull(booking.ProcessedAt);
            Assert.Equal(BookingStatus.Rejected, booking.Status);
        }

        [Fact(DisplayName = "После отклонения брони количество свободных мест события восстанавливается")]
        public async Task BookingServiceReleaseSeatsTest()
        {
            var expectedAvailableSeats = 3;
            var expectedModifySeats = 2;

            var @event = CreateEventAvailableSeats(expectedAvailableSeats);
            var eventRepository = MockEventRepository(@event);
            var eventService = CreateEventService(eventRepository);
            var bookingService = CreateBookingService(eventService);

            var booking = bookingService.CreateBooking(@event.Id);
            Assert.Equal(expectedModifySeats, @event.AvailableSeats);

            bookingService.RejectBooking(booking);
            Assert.Equal(expectedAvailableSeats, @event.AvailableSeats);
        }

        [Fact(DisplayName = "После отклонения брони можно успешно создать новую бронь на то же место")]
        public async Task BookingServiceRejectAndCreateBookingTest()
        {
            var @event = CreateEventAvailableSeats(1);
            var eventRepository = MockEventRepository(@event);
            var eventService = CreateEventService(eventRepository);
            var bookingService = CreateBookingService(eventService);
            var booking = bookingService.CreateBooking(@event.Id);
            bookingService.RejectBooking(booking);
            bookingService.CreateBooking(@event.Id);
        }


        [Theory(DisplayName = "Тест на защиту от овербукинга")]
        // Дано: событие на 5 мест, 20 конкурентных запросов
        // Ожидается: ровно 5 успешных броней, 15 — NoAvailableSeatsException
        [InlineData(5, 20, 5, 15)]
        public async Task OverbookingProtectionTest(int available, int conccurrent,
            int expectedConfirmed, int expectedException)
        {
            var @event = CreateEventAvailableSeats(available);
            var eventRepository = MockEventRepository(@event);
            var eventService = CreateEventService(eventRepository);
            var bookingService = CreateBookingService(eventService);

            var actualException = 0;
            var actualBooking = 0;
            for (int i = 0; i < conccurrent; i++)
            {
                try
                {
                    bookingService.CreateBooking(@event.Id);
                    actualBooking++;
                }
                catch (NoAvailableSeatsException)
                {
                    actualException++;
                }
            }

            Assert.Equal(expectedConfirmed, actualBooking);
            Assert.Equal(expectedException, actualException);
        }

        [Theory(DisplayName = "Тест на уникальность Id при конкурентных запросах")]
        //Дано: событие на 10 мест, 10 одновременных запросов.
        //Ожидается: 10 броней с уникальными Id.
        [InlineData(10, 10, 10)]
        public async Task UniquenessIdCompetitiveQueriesTest(int available, int conccurrent, int expected)
        {
            var @event = CreateEventAvailableSeats(available);
            var eventRepository = MockEventRepository(@event);
            var eventService = CreateEventService(eventRepository);
            var bookingService = CreateBookingService(eventService);

            HashSet<Guid> bookingIds = [];
            for (int i = 0; i < conccurrent; i++)
            {
                bookingIds.Add(bookingService.CreateBooking(@event.Id).Id);
            }

            var actual = bookingIds.Count;
            Assert.Equal(expected, actual);
        }
    }
}
