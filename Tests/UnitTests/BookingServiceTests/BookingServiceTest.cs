using Application.Abstractions;
using Application.Services.BookingService;
using Application.Services.EventService;
using Application.Services.ReservationService;
using Domain.Entities;
using Domain.Exceptions;
using UnitTests.Helpers;
using static UnitTests.Helpers.EntityFactory;

namespace UnitTests.BookingServiceTests
{
    [Trait("Category", "Unit")]
    public class BookingServiceTest : AServiceCollection
    {
        [Fact(DisplayName = "01. Создание брони для существующего события — возвращается BookingInfo со статусом Pending")]
        public async Task CreateBookingTest()
        {
            var personId = Guid.NewGuid();
            var @event = CreateEvent(totalSeats: 1);
            var bookingService = GetInitializedService<IBookingService, Event>(@event);
            var booking = await bookingService.CreateBookingAsync(@event.Id, personId);

            Assert.Equal(@event.Id, booking.EventId);
            Assert.Equal(BookingStatus.Pending, booking.Status);
        }

        [Fact(DisplayName = "02. Создание нескольких броней (до лимита) — все успешны, у каждой уникальный Id")]
        public async Task CreateFewBookingsTest()
        {
            var personId = Guid.NewGuid();
            var @event = CreateEvent(totalSeats: 2);
            var bookingService = GetInitializedService<IBookingService, Event>(@event);

            var booking1 = await bookingService.CreateBookingAsync(@event.Id, personId);
            var booking2 = await bookingService.CreateBookingAsync(@event.Id, personId);

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
            var personId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var bookingService = GetService<IBookingService>();
            await Assert.ThrowsAsync<EventNotFoundException>(async () => await bookingService.CreateBookingAsync(eventId, personId));
        }

        [Fact(DisplayName = "05. Создание брони для удалённого события")]
        public async Task CreateBookingForDeletedEventTest()
        {
            var personId = Guid.NewGuid();
            var @event = CreateEvent();
            var (bookingService, eventService, eventRepository) = 
                GetInitializedServices<IBookingService, IEventService, IEventRepository, Event>(@event);

            Assert.True(await eventService.TryDeleteEventAsync(@event.Id));
            Assert.Null(await eventRepository.GetAsync(@event.Id));

            await Assert.ThrowsAsync<EventNotFoundException>(async () => await bookingService.CreateBookingAsync(@event.Id, personId));
        }

        [Fact(DisplayName = "06. Получение брони по несуществующему Id")]
        public async Task GetNotExitedBookingTest()
        {
            var bookingService = GetService<IBookingService>();

            await Assert.ThrowsAsync<BookingNotFoundException>(async () => await bookingService.GetBookingByIdAsync(Guid.NewGuid()));
        }

        [Fact(DisplayName = "07. Создание брони уменьшает AvailableSeats на 1")]
        public async Task CreateBookingDecreaseAvailableSeatsByOneTest()
        {
            var personId = Guid.NewGuid();
            var initialSeats = 2;
            var @event = CreateEvent(totalSeats: initialSeats);
            var bookingService = GetInitializedService<IBookingService, Event>(@event);

            // Бронируем пока есть места
            for (int expected = initialSeats; expected > 0;)
            {
                await bookingService.CreateBookingAsync(@event.Id, personId);
                Assert.Equal(--expected, @event.AvailableSeats);
            }
        }

        [Fact(DisplayName = "08. Бронирование при отсутствии|исчерпании мест → NoAvailableSeatsException")]
        public async Task CreateBookingNoAvailableSeatsExceptionTest()
        {
            var personId = Guid.NewGuid();
            var @event = CreateEvent(totalSeats: 0);
            var bookingService = GetInitializedService<IBookingService, Event>(@event);

            await Assert.ThrowsAsync<NoAvailableSeatsException>(async () => await bookingService.CreateBookingAsync(@event.Id, personId));
        }

        [Fact(DisplayName = "09. После подтверждения бронь возвращает статус Confirmed и заполненный ProcessedAt")]
        public async Task BookingServiceConfirmBookingTest()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            Initialize(CreateEvent(eventId: eventId, totalSeats: 1));
            var booking = CreateBooking(eventId: eventId);
            Initialize(booking);
            var bookingService = GetService<IBookingService>();

            // Act
            await bookingService.ConfirmBookingAsync(booking);

            // Assert
            Assert.NotNull(booking.ProcessedAt);
            Assert.Equal(BookingStatus.Confirmed, booking.Status);
        }

        [Fact(DisplayName = "10. После отклонения бронь возвращает статус Rejected и заполненный ProcessedAt")]
        public async Task BookingServiceRejectBookingTest()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            Initialize(CreateEvent(eventId: eventId, totalSeats: 1));
            var booking = CreateBooking(eventId: eventId);
            Initialize(booking);
            var bookingService = GetService<IBookingService>();

            // Act
            await bookingService.RejectBookingAsync(booking);

            // Assert
            Assert.NotNull(booking.ProcessedAt);
            Assert.Equal(BookingStatus.Rejected, booking.Status);
        }

        [Fact(DisplayName = "11. После отклонения брони количество свободных мест события восстанавливается")]
        public async Task BookingServiceReleaseSeatsTest()
        {
            var personId = Guid.NewGuid();
            var expectedAvailableSeats = 3;
            var expectedModifySeats = 2;

            var @event = CreateEvent(totalSeats: expectedAvailableSeats);
            var bookingService = GetInitializedService<IBookingService, Event>(@event);

            var booking = await bookingService.CreateBookingAsync(@event.Id, personId);
            Assert.Equal(expectedModifySeats, @event.AvailableSeats);

            await bookingService.RejectBookingAsync(booking.BuildBooking());
            Assert.Equal(expectedAvailableSeats, @event.AvailableSeats);
        }

        [Fact(DisplayName = "12. После отклонения брони можно успешно создать новую бронь на то же место")]
        public async Task BookingServiceRejectAndCreateBookingTest()
        {
            var personId = Guid.NewGuid();
            var @event = CreateEvent(totalSeats: 1);
            var bookingService = GetInitializedService<IBookingService, Event>(@event);

            var booking = await bookingService.CreateBookingAsync(@event.Id, personId);
            await bookingService.RejectBookingAsync(booking.BuildBooking());
            await bookingService.CreateBookingAsync(@event.Id, personId);
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
            var personId = Guid.NewGuid();
            var @event = CreateEvent(totalSeats: available);
            var bookingService = GetInitializedService<IBookingService, Event>(@event);

            async Task<bool> TryCreateBookingAsync(Guid eventId)
            {
                try
                {
                    await bookingService.CreateBookingAsync(eventId, personId);
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
            var personId = Guid.NewGuid();
            var @event = CreateEvent(totalSeats: available);
            var bookingService = GetInitializedService<IBookingService, Event>(@event);

            var concurrentTask = Enumerable.Range(0, concurrent)
                .Select(_ => bookingService.CreateBookingAsync(@event.Id, personId));

            var actual = (await Task.WhenAll(concurrentTask))
                .Select(a => a.Id).ToHashSet()
                .Count;

            Assert.Equal(expected, actual);
        }


        [Fact(DisplayName = "15. Попытка забронировать прошедшее событие приводит к ошибке")]
        public async Task CreateExpiredBookingTest()
        {
            var personId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var @event = CreateEvent(
                eventId: eventId,
                startAt: DateTime.Now.AddHours(-2),
                endAt: DateTime.Now.AddHours(-1));

            var bookingService = GetInitializedService<IBookingService, Event>(@event);

            await Assert.ThrowsAsync<PastEventBookingException>(async () => await bookingService.CreateBookingAsync(eventId, personId));
        }

        [Fact(DisplayName = "16. При достижении лимита активных броней новая бронь не создаётся")]
        public async Task CreateOverLimitBookingTest()
        {
            var personId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            Initialize(CreatePerson(personId: personId));
            Initialize(CreateEvent(eventId: eventId, totalSeats: 11));
            foreach (var _ in Enumerable.Range(1, IReservationService.PersonMaxBookingCount))
            {
                Initialize(CreateBooking(eventId: eventId, personId: personId));
            }
            var bookingService = GetInitializedService<IBookingService, Event>();

            await Assert.ThrowsAsync<ActiveBookingLimitException>(async () => await bookingService.CreateBookingAsync(eventId, personId));
        }

        [Fact(DisplayName = "17. Лимиты разных пользователей не влияют друг на друга")]
        public async Task CreateDiffPersonLimitBookingTest()
        {
            var personId1 = Guid.NewGuid();
            var personId2 = Guid.NewGuid();
            var personId3 = Guid.NewGuid();
            Initialize(
                CreatePerson(personId: personId1),
                CreatePerson(personId: personId2),
                CreatePerson(personId: personId3));
            var eventId = Guid.NewGuid();
            Initialize(CreateEvent(eventId: eventId, totalSeats: 31));

            var bookingService = GetService<IBookingService>();

            // Person 1
            foreach (var _ in Enumerable.Range(1, IReservationService.PersonMaxBookingCount))
            {
                await bookingService.CreateBookingAsync(eventId, personId1);
            }

            await Assert.ThrowsAsync<ActiveBookingLimitException>(async () => await bookingService.CreateBookingAsync(eventId, personId1));

            // Person 2
            foreach (var _ in Enumerable.Range(1, IReservationService.PersonMaxBookingCount))
            {
                await bookingService.CreateBookingAsync(eventId, personId2);
            }

            await Assert.ThrowsAsync<ActiveBookingLimitException>(async () => await bookingService.CreateBookingAsync(eventId, personId2));

            // Person 3
            foreach (var _ in Enumerable.Range(1, IReservationService.PersonMaxBookingCount))
            {
                await bookingService.CreateBookingAsync(eventId, personId3);
            }

            await Assert.ThrowsAsync<ActiveBookingLimitException>(async () => await bookingService.CreateBookingAsync(eventId, personId3));
        }

        [Fact(DisplayName = "18. Dладелец может отменить свою бронь")]
        public async Task CancelBookingWithOwnerTest()
        {
            // Arrange
            var personId = Guid.NewGuid();
            Initialize(CreatePerson(personId: personId));
            var eventId = Guid.NewGuid();
            Initialize(CreateEvent(eventId: eventId, totalSeats: 3));
            var bookingId = Guid.NewGuid();
            Initialize(CreateBooking(bookingId: bookingId, eventId: eventId, personId: personId));
            var bookingService = GetService<IBookingService>();

            // Action
            await bookingService.CancelBookingAsync(bookingId, personId, PersonRole.User);

            // Assert
            await Assert.ThrowsAsync<BookingNotFoundException>(async () => await bookingService.GetBookingByIdAsync(bookingId));
        }

        [Fact(DisplayName = "19. Администратор может отменить любую бронь")]
        public async Task CancelBookingWithAdministratorTest()
        {
            // Arrange
            var personId1 = Guid.NewGuid();
            var personId2 = Guid.NewGuid();
            Initialize(
                CreatePerson(personId: personId1),
                CreatePerson(personId: personId2));
            var eventId = Guid.NewGuid();
            Initialize(CreateEvent(eventId: eventId, totalSeats: 3));
            var bookingId = Guid.NewGuid();
            Initialize(CreateBooking(bookingId: bookingId, eventId: eventId, personId: personId1));
            var bookingService = GetService<IBookingService>();

            // Action
            await bookingService.CancelBookingAsync(bookingId, personId2, PersonRole.Admin);

            // Assert
            await Assert.ThrowsAsync<BookingNotFoundException>(async () => await bookingService.GetBookingByIdAsync(bookingId));
        }

        [Fact(DisplayName = "20. Обычный пользователь не может отменить чужую")]
        public async Task CancelBookingWithUnauthorizedBookingOperationExceptionTest()
        {
            var personId1 = Guid.NewGuid();
            var personId2 = Guid.NewGuid();
            Initialize(
                CreatePerson(personId: personId1),
                CreatePerson(personId: personId2));
            var eventId = Guid.NewGuid();
            Initialize(CreateEvent(eventId: eventId, totalSeats: 3));
            var bookingId = Guid.NewGuid();

            Initialize(CreateBooking(bookingId: bookingId, eventId: eventId, personId: personId1));

            var bookingService = GetService<IBookingService>();

            await Assert.ThrowsAsync<UnauthorizedBookingOperationException>(async () => await bookingService.CancelBookingAsync(bookingId, personId2, PersonRole.User));
        }
    }
}
