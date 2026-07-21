using Domain.Entities;
using IntegrationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using static IntegrationTests.Helpers.EntityFactory;

namespace IntegrationTests.RepositoriesTests
{
    /// <summary>
    /// Все методы репозитория бронирований
    /// </summary>
    [Collection("SequentialTests")]
    public class BookingRepositoryTests : ADockerDB
    {
        [Fact(DisplayName = "01. Создать бронирование")]
        public async Task CreateAsyncTest()
        {
            // Arrange
            await using var context = CreateContext();
            var eventId = Guid.NewGuid();
            var @event = CreateEvent(eventId);
            context.Events.Add(@event);
            var personId = Guid.NewGuid();
            var person = CreatePerson(personId);
            context.Persons.Add(person);
            await context.SaveChangesAsync();

            var bookingId = Guid.NewGuid();
            var expected = CreateBooking(id: bookingId, eventId: @event.Id, personId: personId);
            var repository = CreateBookingRepository(context);

            // Act
            await repository.CreateAsync(expected);

            // Assert
            await using var verifycontext = CreateContext();
            var actual = await verifycontext.Bookings.FirstOrDefaultAsync(a => a.Id == bookingId);

            Assert.NotNull(actual);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.EventId, actual.EventId);
            Assert.Equal(expected.PersonId, actual.PersonId);
            Assert.Equal(expected.Status, actual.Status);
            Assert.Equal(expected.CreatedAt, actual.CreatedAt, TimeSpan.FromMilliseconds(1));
            Assert.Null(actual.ProcessedAt);
        }

        [Fact(DisplayName = "02. Получить бронирование")]
        public async Task GetAsyncTest()
        {
            // Arrange
            await using var context = CreateContext();
            var eventId = Guid.NewGuid();
            var @event = CreateEvent(eventId);
            context.Events.Add(@event);
            var personId = Guid.NewGuid();
            var person = CreatePerson(personId);
            context.Persons.Add(person);
            var bookingId = Guid.NewGuid();
            var expected = CreateBooking(id: bookingId, eventId: @event.Id, personId: personId, processedAt: DateTime.Now.AddHours(1));
            context.Bookings.Add(expected);
            await context.SaveChangesAsync();

            await using var verifyContext = CreateContext();
            var repository = CreateBookingRepository(verifyContext);

            // Act
            var actual = await repository.GetAsync(bookingId);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.EventId, actual.EventId);
            Assert.Equal(expected.PersonId, actual.PersonId);
            Assert.Equal(expected.Status, actual.Status);
            Assert.Equal(expected.CreatedAt, actual.CreatedAt, TimeSpan.FromMilliseconds(1));
            Assert.Equal(expected.ProcessedAt!.Value, actual.ProcessedAt!.Value, TimeSpan.FromMilliseconds(1));
        }

        [Theory(DisplayName = "03. Пагинация событий некорректное количество событий на странице")]
        [InlineData(BookingStatus.Pending)]
        [InlineData(BookingStatus.Confirmed)]
        [InlineData(BookingStatus.Rejected)]
        public async Task GetBookingsByStatusAsyncTest(BookingStatus status)
        {
            // Arrange
            await using var context = CreateContext();
            var eventId = Guid.NewGuid();
            var @event = CreateEvent(eventId, totalSeats: 5);
            context.Events.Add(@event);
            var personId = Guid.NewGuid();
            var person = CreatePerson(personId);
            context.Persons.Add(person);
            var bookingId = Guid.NewGuid();
            var expectedItem = CreateBooking(id: bookingId, eventId: @event.Id, personId: personId, status: status);
            context.Bookings.Add(expectedItem);
            context.Bookings.Add(CreateBooking(eventId: eventId, personId: personId, status: BookingStatus.Pending));
            context.Bookings.Add(CreateBooking(eventId: eventId, personId: personId, status: BookingStatus.Confirmed));
            context.Bookings.Add(CreateBooking(eventId: eventId, personId: personId, status: BookingStatus.Rejected));
            await context.SaveChangesAsync();

            await using var verifyContext = CreateContext();
            var repository = CreateBookingRepository(verifyContext);

            // Act
            var actualCollection = await repository.GetBookingsByStatus(status);

            // Assert
            Assert.Equal(2, actualCollection.Count());
            Assert.Contains(actualCollection, a => a.Id == bookingId);
        }


        [Fact]
        public async Task TryUpdateAsyncTest()
        {
            // Arrange
            await using var context = CreateContext();
            var eventId = Guid.NewGuid();
            var @event = CreateEvent(eventId);
            context.Events.Add(@event);
            var personId = Guid.NewGuid();
            var person = CreatePerson(personId);
            context.Persons.Add(person);
            var bookingId = Guid.NewGuid();
            var booking = CreateBooking(id: bookingId,
                eventId: @event.Id,
                personId: personId,
                status: BookingStatus.Pending,
                createdAt: DateTime.Now);
            context.Bookings.Add(booking);
            await context.SaveChangesAsync();

            await using var updateContext = CreateContext();
            var repository = CreateBookingRepository(updateContext);
            var expected = CreateBooking(id: bookingId,
                eventId: @event.Id,
                personId: personId,
                status: BookingStatus.Confirmed,
                createdAt: DateTime.Now,
                processedAt: DateTime.Now);

            // Act
            var actualCollection = await repository.TryUpdateAsync(expected);

            // Assert
            await using var verifyContext = CreateContext();
            var actual = verifyContext.Bookings.Single(a => a.Id == bookingId);

            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.EventId, actual.EventId);
            Assert.Equal(expected.PersonId, actual.PersonId);
            Assert.Equal(expected.Status, actual.Status);
            Assert.Equal(expected.CreatedAt, actual.CreatedAt, TimeSpan.FromMilliseconds(1));
            Assert.Equal(expected.ProcessedAt!.Value, actual.ProcessedAt!.Value, TimeSpan.FromMilliseconds(1));
        }

        [Fact]
        public async Task TryRemoveAsyncTest()
        {
            // Arrange
            await using var context = CreateContext();
            var eventId = Guid.NewGuid();
            var @event = CreateEvent(eventId);
            context.Events.Add(@event);
            var personId = Guid.NewGuid();
            var person = CreatePerson(personId);
            context.Persons.Add(person);
            var bookingId = Guid.NewGuid();
            var booking = CreateBooking(id: bookingId, eventId: @event.Id, personId: personId);
            context.Bookings.Add(booking);
            context.SaveChanges();

            await using var deleteContext = CreateContext();
            var repository = CreateBookingRepository(deleteContext);

            // Act
            var deletedCount = await repository.TryRemoveAsync(bookingId);

            // Assert
            Assert.Equal(1, deletedCount);

            await using var verifyContext = CreateContext();
            Assert.DoesNotContain(await verifyContext.Bookings.ToListAsync(), a => a.Id == bookingId);
        }
    }
}
