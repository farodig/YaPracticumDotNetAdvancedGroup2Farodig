using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests
{
    [Trait("Category", "Unit")]
    internal class SagaOrEventTests
    {
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

            await Assert.ThrowsAsync<PastEventReserveException>(async () => await bookingService.CreateBookingAsync(eventId, personId));
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

        [Fact(DisplayName = "01. Проверка успешной обработки бронирования события")]
        public async Task ProcessSuccessBookingEventTest()
        {
            var booking = CreateBooking();
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

        //public static Person CreatePerson(Guid? personId = null) => new()
        //{
        //    Id = personId ?? Guid.NewGuid(),
        //};

        //public static Booking BuildBooking(this BookingResponse data) => new()
        //{
        //    Id = data.Id,
        //    EventId = data.EventId,
        //    Status = data.Status,
        //    CreatedAt = data.CreatedAt,
        //    ProcessedAt = data.ProcessedAt,
        //};
    }
}
