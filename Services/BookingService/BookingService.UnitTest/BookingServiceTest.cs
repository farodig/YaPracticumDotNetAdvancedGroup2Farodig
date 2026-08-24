using BookingService.Application.Abstractions;
using BookingService.Domain.Entities;
using BookingService.Domain.Exceptions;
using BookingService.UnitTest.Helpers;
using TokenService.Exceptions;
using static BookingService.UnitTest.Helpers.EntityFactory;

namespace BookingService.UnitTest
{
    [Trait("Category", "Unit")]
    public class BookingServiceTest : AServiceCollection
    {
        [Fact(DisplayName = "01. Создание брони для существующего события — возвращается BookingInfo со статусом Pending")]
        public async Task CreateBookingTest()
        {
            // Arrange
            var personId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var bookingService = GetService<IBookingService>();

            // Action
            var booking = await bookingService.CreateBookingAsync(eventId, personId);

            // Assert
            Assert.Equal(eventId, booking.EventId);
            Assert.Equal(BookingStatus.Pending, booking.Status);
        }

        [Fact(DisplayName = "02. Создание нескольких броней (до лимита) — все успешны, у каждой уникальный Id")]
        public async Task CreateFewBookingsTest()
        {
            // Arrange
            var personId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var bookingService = GetService<IBookingService>();

            // Act
            var booking1 = await bookingService.CreateBookingAsync(eventId, personId);
            var booking2 = await bookingService.CreateBookingAsync(eventId, personId);

            // Assert
            Assert.Equal(eventId, booking1.EventId);
            Assert.Equal(eventId, booking2.EventId);
            Assert.NotEqual(booking1.Id, booking2.Id);
        }

        [Fact(DisplayName = "03. Получение брони по Id — возвращается корректная информация")]
        public async Task GetBookingByIdAsyncTest()
        {
            // Arrange
            var expectedBooking = CreateBooking();
            var bookingService = GetInitializedService<IBookingService, Booking>(expectedBooking);

            // Act
            var actualBooking = await bookingService.GetBookingByIdAsync(expectedBooking.Id);

            // Assert
            Assert.NotNull(actualBooking);
            Assert.Equal(expectedBooking.Id, actualBooking.Id);
        }

        [Fact(DisplayName = "04. Получение брони по несуществующему Id")]
        public async Task GetNotExitedBookingTest()
        {
            // Arrange
            var bookingService = GetService<IBookingService>();

            // Assert
            await Assert.ThrowsAsync<BookingNotFoundException>(async () => await bookingService.GetBookingByIdAsync(Guid.NewGuid()));
        }

        [Fact(DisplayName = "05. После подтверждения бронь возвращает статус Confirmed и заполненный ProcessedAt")]
        public async Task BookingServiceConfirmBookingTest()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var booking = CreateBooking(eventId: eventId);
            Initialize(booking);
            var bookingService = GetService<IBookingService>();

            // Act
            await bookingService.ConfirmBookingAsync(booking);

            // Assert
            Assert.NotNull(booking.ProcessedAt);
            Assert.Equal(BookingStatus.Confirmed, booking.Status);
        }

        [Fact(DisplayName = "06. После отклонения бронь возвращает статус Rejected и заполненный ProcessedAt")]
        public async Task BookingServiceRejectBookingTest()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var booking = CreateBooking(eventId: eventId);
            Initialize(booking);
            var bookingService = GetService<IBookingService>();

            // Act
            await bookingService.RejectBookingAsync(booking);

            // Assert
            Assert.NotNull(booking.ProcessedAt);
            Assert.Equal(BookingStatus.Rejected, booking.Status);
        }

        [Theory(DisplayName = "07. Тест на уникальность Id при конкурентных запросах")]
        //Дано: 10 одновременных запросов.
        //Ожидается: 10 броней с уникальными Id.
        [InlineData(10, 10)]
        public async Task UniquenessIdCompetitiveQueriesTest(int concurrent, int expected)
        {
            // Arrange
            var personId = Guid.NewGuid();
            var @eventId = Guid.NewGuid();
            var bookingService = GetService<IBookingService>();

            // Act
            var concurrentTask = Enumerable.Range(0, concurrent)
                .Select(_ => bookingService.CreateBookingAsync(@eventId, personId));

            var actual = (await Task.WhenAll(concurrentTask))
                .Select(a => a.Id).ToHashSet()
                .Count;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact(DisplayName = "08. Лимиты разных пользователей не влияют друг на друга")]
        public async Task CreateDiffPersonLimitBookingTest()
        {
            // Arrange
            var personId1 = Guid.NewGuid();
            var personId2 = Guid.NewGuid();
            var personId3 = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var bookingService = GetService<IBookingService>();

            // Person 1
            foreach (var _ in Enumerable.Range(1, IBookingService.PersonMaxBookingCount))
            {
                await bookingService.CreateBookingAsync(eventId, personId1);
            }

            await Assert.ThrowsAsync<ActiveBookingLimitException>(async () => await bookingService.CreateBookingAsync(eventId, personId1));

            // Person 2
            foreach (var _ in Enumerable.Range(1, IBookingService.PersonMaxBookingCount))
            {
                await bookingService.CreateBookingAsync(eventId, personId2);
            }

            await Assert.ThrowsAsync<ActiveBookingLimitException>(async () => await bookingService.CreateBookingAsync(eventId, personId2));

            // Person 3
            foreach (var _ in Enumerable.Range(1, IBookingService.PersonMaxBookingCount))
            {
                await bookingService.CreateBookingAsync(eventId, personId3);
            }

            await Assert.ThrowsAsync<ActiveBookingLimitException>(async () => await bookingService.CreateBookingAsync(eventId, personId3));
        }

        [Fact(DisplayName = "09. Dладелец может отменить свою бронь")]
        public async Task CancelBookingWithOwnerTest()
        {
            // Arrange
            var personId = Guid.NewGuid();
            var bookingId = Guid.NewGuid();
            Initialize(CreateBooking(bookingId: bookingId, personId: personId, status: BookingStatus.Confirmed));
            var bookingService = GetService<IBookingService>();

            // Action
            await bookingService.CancelBookingByPersonAsync(bookingId, personId);

            // Assert
            await Assert.ThrowsAsync<BookingNotFoundException>(async () => await bookingService.GetBookingByIdAsync(bookingId));
        }

        [Fact(DisplayName = "10. Администратор может отменить любую бронь")]
        public async Task CancelBookingWithAdministratorTest()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            Initialize(CreateBooking(bookingId: bookingId, status: BookingStatus.Confirmed));
            var bookingService = GetService<IBookingService>();

            // Action
            await bookingService.CancelBookingByAdminAsync(bookingId);

            // Assert
            await Assert.ThrowsAsync<BookingNotFoundException>(async () => await bookingService.GetBookingByIdAsync(bookingId));
        }

        [Fact(DisplayName = "11. Обычный пользователь не может отменить чужую")]
        public async Task CancelBookingWithUnauthorizedBookingOperationExceptionTest()
        {
            // Arrange
            var personId1 = Guid.NewGuid();
            var personId2 = Guid.NewGuid();
            var bookingId = Guid.NewGuid();
            Initialize(CreateBooking(bookingId: bookingId, personId: personId1));
            var bookingService = GetService<IBookingService>();

            // Assert
            await Assert.ThrowsAsync<UnauthorizedBookingOperationException>(async () => await bookingService.CancelBookingByPersonAsync(bookingId, personId2));
        }

        [Theory(DisplayName = "12.  Нельзя отменить не обработанную бронь")]
        //[InlineData(BookingStatus.Cancelled)] // TODO отменённые в фильтре бд, возможно этот фильтр не нужен, пересмотреть, подумать
        [InlineData(BookingStatus.Pending)]
        [InlineData(BookingStatus.Rejected)]
        public async Task UnableToCancelNotConfirmedBookingTest(BookingStatus status)
        {
            // Arrange
            var personId = Guid.NewGuid();
            var bookingId = Guid.NewGuid();
            Initialize(CreateBooking(bookingId: bookingId, personId: personId, status: status));
            var bookingService = GetService<IBookingService>();

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await bookingService.CancelBookingByPersonAsync(bookingId, personId));
        }
    }
}