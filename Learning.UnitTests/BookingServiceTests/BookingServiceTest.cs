using Learning.UnitTests.Helpers;
using LearningWebApi.Entities;
using LearningWebApi.Exceptions;
using LearningWebApi.Repositories;
using LearningWebApi.Services.BookingService;
using LearningWebApi.Services.EventService;
using static Learning.UnitTests.Helpers.EntityFactory;

namespace Learning.UnitTests.BookingServiceTests
{
    [Trait("Category", "Unit")]
    public class BookingServiceTest : AServiceCollection
    {
        [Fact(DisplayName = "01. Создание брони для существующего события — возвращается BookingInfo со статусом Pending")]
        public async Task CreateBookingTest()
        {
            var @event = CreateEventAvailableSeats();
            var bookingService = GetInitializedService<IBookingService, Event>(@event);
            var booking = await bookingService.CreateBookingAsync(@event.Id);

            Assert.Equal(@event.Id, booking.EventId);
            Assert.Equal(BookingStatus.Pending, booking.Status);
        }

        [Fact(DisplayName = "02. Создание нескольких броней (до лимита) — все успешны, у каждой уникальный Id")]
        public async Task CreateFewBookingsTest()
        {
            var @event = CreateEventAvailableSeats(2);
            var bookingService = GetInitializedService<IBookingService, Event>(@event);

            var booking1 = await bookingService.CreateBookingAsync(@event.Id);
            var booking2 = await bookingService.CreateBookingAsync(@event.Id);

            Assert.Equal(@event.Id, booking1.EventId);
            Assert.Equal(@event.Id, booking2.EventId);
            Assert.NotEqual(booking1.Id, booking2.Id);
        }

        [Fact(DisplayName = "03. Получение брони по Id — возвращается корректная информация")]
        public async Task GetBookingByIdAsyncTest()
        {
            var expectedBooking = CreateBooking();
            var bookingService = GetInitializedService<IBookingService, Booking>(expectedBooking);

            var actualBooking = await bookingService.GetBookingByIdAsync(expectedBooking.Id);

            Assert.NotNull(actualBooking);
            Assert.Equal(expectedBooking.Id, actualBooking.Id);
        }

        [Fact(DisplayName = "04. Бронирование для несуществующего события → NotFoundException")]
        public async Task CreateBookingNotFoundExceptionTest()
        {
            var bookingService = GetService<IBookingService>();
            await Assert.ThrowsAsync<EventNotFoundException>(async () => await bookingService.CreateBookingAsync(Guid.NewGuid()));
        }

        [Fact(DisplayName = "05. Создание брони для удалённого события")]
        public async Task CreateBookingForDeletedEventTest()
        {
            var @event = CreateEvent();
            var (bookingService, eventService, eventRepository) = 
                GetInitializedServices<IBookingService, IEventService, IEventRepository, Event>(@event);

            Assert.True(await eventService.TryDeleteEventAsync(@event.Id));
            Assert.Null(await eventRepository.GetAsync(@event.Id));

            await Assert.ThrowsAsync<EventNotFoundException>(async () => await bookingService.CreateBookingAsync(@event.Id));
        }

        [Fact(DisplayName = "06. Получение брони по несуществующему Id")]
        public async Task GetNotExitedBookingTest()
        {
            var bookingService = GetService<IBookingService>();

            Assert.Null(await bookingService.GetBookingByIdAsync(Guid.NewGuid()));
        }

        [Fact(DisplayName = "07. Создание брони уменьшает AvailableSeats на 1")]
        public async Task CreateBookingDecreaseAvailableSeatsByOneTest()
        {
            var initialSeats = 2;
            var @event = CreateEventAvailableSeats(initialSeats);
            var bookingService = GetInitializedService<IBookingService, Event>(@event);

            // Бронируем пока есть места
            for (int expected = initialSeats; expected > 0;)
            {
                await bookingService.CreateBookingAsync(@event.Id);
                Assert.Equal(--expected, @event.AvailableSeats);
            }
        }

        [Fact(DisplayName = "08. Бронирование при отсутствии|исчерпании мест → NoAvailableSeatsException")]
        public async Task CreateBookingNoAvailableSeatsExceptionTest()
        {
            var @event = CreateEventAvailableSeats(0);
            var bookingService = GetInitializedService<IBookingService, Event>(@event);

            await Assert.ThrowsAsync<NoAvailableSeatsException>(async () => await bookingService.CreateBookingAsync(@event.Id));
        }

        [Fact(DisplayName = "09. После подтверждения бронь возвращает статус Confirmed и заполненный ProcessedAt")]
        public async Task BookingServiceConfirmBookingTest()
        {
            var @event = CreateEventAvailableSeats();
            var bookingService = GetInitializedService<IBookingService, Event>(@event);

            var booking = await bookingService.CreateBookingAsync(@event.Id);
            await bookingService.ConfirmBookingAsync(booking);

            Assert.NotNull(booking.ProcessedAt);
            Assert.Equal(BookingStatus.Confirmed, booking.Status);
        }

        [Fact(DisplayName = "10. После отклонения бронь возвращает статус Rejected и заполненный ProcessedAt")]
        public async Task BookingServiceRejectBookingTest()
        {
            var @event = CreateEventAvailableSeats();
            var bookingService = GetInitializedService<IBookingService, Event>(@event);

            var booking = await bookingService.CreateBookingAsync(@event.Id);
            await bookingService.RejectBookingAsync(booking);

            Assert.NotNull(booking.ProcessedAt);
            Assert.Equal(BookingStatus.Rejected, booking.Status);
        }

        [Fact(DisplayName = "11. После отклонения брони количество свободных мест события восстанавливается")]
        public async Task BookingServiceReleaseSeatsTest()
        {
            var expectedAvailableSeats = 3;
            var expectedModifySeats = 2;

            var @event = CreateEventAvailableSeats(expectedAvailableSeats);
            var bookingService = GetInitializedService<IBookingService, Event>(@event);

            var booking = await bookingService.CreateBookingAsync(@event.Id);
            Assert.Equal(expectedModifySeats, @event.AvailableSeats);

            await bookingService.RejectBookingAsync(booking);
            Assert.Equal(expectedAvailableSeats, @event.AvailableSeats);
        }

        [Fact(DisplayName = "12. После отклонения брони можно успешно создать новую бронь на то же место")]
        public async Task BookingServiceRejectAndCreateBookingTest()
        {
            var @event = CreateEventAvailableSeats(1);
            var bookingService = GetInitializedService<IBookingService, Event>(@event);

            var booking = await bookingService.CreateBookingAsync(@event.Id);
            await bookingService.RejectBookingAsync(booking);
            await bookingService.CreateBookingAsync(@event.Id);
        }


        [Theory(DisplayName = "13. Тест на защиту от овербукинга")]
        // Дано: событие на 5 мест, 20 конкурентных запросов
        // Ожидается: ровно 5 успешных броней, 15 — NoAvailableSeatsException
        [InlineData(5, 20, 5, 15)]
        // AvailableSeats == 0 после гонки - это усилит покрытие.
        [InlineData(0, 20, 0, 20)]
        public async Task OverbookingProtectionTest(int available, int concurrent,
            int expectedConfirmed, int expectedException)
        {
            var @event = CreateEventAvailableSeats(available);
            var bookingService = GetInitializedService<IBookingService, Event>(@event);

            async Task<bool> TryCreateBookingAsync(Guid eventId)
            {
                try
                {
                    await bookingService.CreateBookingAsync(eventId);
                    return true;
                }
                catch (NoAvailableSeatsException)
                {
                    return false;
                }
            }

            var concurrentTask = Enumerable.Repeat(0, concurrent)
                .Select(_ => TryCreateBookingAsync(@event.Id));

            var (actualConfirmed, actualException) = (await Task.WhenAll(concurrentTask)).Aggregate((Success: 0, Failure: 0),
                (acc, x) => x
                ? (acc.Success + 1, acc.Failure)
                : (acc.Success, acc.Failure + 1));

            Assert.Equal(expectedConfirmed, actualConfirmed);
            Assert.Equal(expectedException, actualException);
        }

        [Theory(DisplayName = "14. Тест на уникальность Id при конкурентных запросах")]
        //Дано: событие на 10 мест, 10 одновременных запросов.
        //Ожидается: 10 броней с уникальными Id.
        [InlineData(10, 10, 10)]
        public async Task UniquenessIdCompetitiveQueriesTest(int available, int concurrent, int expected)
        {
            var @event = CreateEventAvailableSeats(available);
            var bookingService = GetInitializedService<IBookingService, Event>(@event);

            var concurrentTask = Enumerable.Range(0, concurrent)
                .Select(_ => bookingService.CreateBookingAsync(@event.Id));

            var actual = (await Task.WhenAll(concurrentTask))
                .Select(a => a.Id).ToHashSet()
                .Count;

            Assert.Equal(expected, actual);
        }
    }
}
