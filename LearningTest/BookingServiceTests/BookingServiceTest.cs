using LearningWebApi.Entities;
using LearningWebApi.Repositories;
using LearningWebApi.Services.BookingService;
using Moq;
using static LearningTest.Factories.EntityFactory;
using static LearningTest.Factories.ServiceFactory;

namespace LearningTest.BookingServiceTests
{
    public class BookingServiceTest
    {
        [Fact(DisplayName = "создание брони для существующего события — возвращается BookingInfo со статусом Pending")]
        public async Task CreateBookingTest()
        {
            var mockEventRepository = new Mock<IEventRepository>();

            var @event = new Event()
            {
                Id = Guid.NewGuid(),
                AvailableSeats = 1,
            };

            mockEventRepository
                .Setup(repo => repo.TryGetValue(@event.Id, out @event))
                .Returns(true);

            var bookingService = CreateBookingServiceWithEventRepository(mockEventRepository.Object);
            var booking = bookingService.CreateBooking(@event.Id);

            Assert.NotNull(booking);
            Assert.Equal(@event.Id, booking.EventId);
            Assert.Equal(BookingStatus.Pending, booking.Status);
        }

        [Fact(DisplayName = "создание нескольких броней для одного события — все создаются с уникальными Id")]
        public async Task CreateBookingTheSameEventTest()
        {
            var mockEventRepository = new Mock<IEventRepository>();

            var @event = new Event()
            {
                Id = Guid.NewGuid(),
                AvailableSeats = 1,
            };

            mockEventRepository
                .Setup(repo => repo.TryGetValue(@event.Id, out @event))
                .Returns(true);

            var bookingService = CreateBookingServiceWithEventRepository(mockEventRepository.Object);

            var booking1 = bookingService.CreateBooking(@event.Id);
            var booking2 = bookingService.CreateBooking(@event.Id);

            Assert.NotNull(booking1);
            Assert.NotNull(booking2);

            Assert.Equal(@event.Id, booking1.EventId);
            Assert.Equal(@event.Id, booking2.EventId);

            Assert.NotEqual(booking1.Id, booking2.Id);
        }

        [Fact(DisplayName = "получение брони по Id — возвращается корректная информация")]
        public async Task GetBookingByIdAsyncTest()
        {
            var mockBookingRepository = new Mock<IBookingRepository>();
            var expectedBooking = new Booking { Id = Guid.NewGuid(), };
            mockBookingRepository
                .Setup(repo => repo.TryGetValue(It.IsAny<Guid>(), out expectedBooking))
                .Returns(true);

            var bookingService = CreateBookingServiceWithBookingRepository(mockBookingRepository.Object);

            var actualBooking = bookingService.GetBookingById(expectedBooking.Id);

            Assert.NotNull(actualBooking);
            Assert.Equal(expectedBooking.Id, actualBooking.Id);
        }

        [Fact(DisplayName = "получение брони отражает изменение статуса (после Confirm/Reject)")]
        public async Task ChangeBookingStatusFromPendingToConfirmedTest()
        {
            // Создаём мок-объект события
            var mockEventRepository = new Mock<IEventRepository>();
            var @event = new Event()
            {
                Id = Guid.NewGuid(),
                AvailableSeats = 1,
            };

            mockEventRepository
                .Setup(repo => repo.TryGetValue(@event.Id, out @event))
                .Returns(true);

            var bookingRepository = new BookingRepository();
            var bookingService = CreateBookingService(bookingRepository, mockEventRepository.Object);
            using (var bookingProcessor = new BookingProcessor(bookingRepository))
            {
                var booking = bookingService.CreateBooking(@event.Id);
                Assert.NotNull(booking);
                Assert.Equal(BookingStatus.Pending, booking.Status);

                booking = bookingService.GetBookingById(booking.Id);
                Assert.NotNull(booking);
                Assert.Equal(BookingStatus.Pending, booking.Status);

                await bookingProcessor.StartAsync(CancellationToken.None);
                await Task.Delay(TimeSpan.FromSeconds(3));

                booking = bookingService.GetBookingById(booking.Id);
                Assert.NotNull(booking);
                Assert.Equal(BookingStatus.Confirmed, booking.Status);

                await bookingProcessor.StopAsync(CancellationToken.None);
            }
        }

        [Fact(DisplayName = "создание брони для несуществующего события")]
        public async Task CreateBookingForNotExistedEventTest()
        {
            var bookingService = CreateBookingService();
            var booking = bookingService.CreateBooking(Guid.NewGuid());
            Assert.Null(booking);
        }

        [Fact(DisplayName = "создание брони для удалённого события")]
        public async Task CreateBookingForDeletedEventTest()
        {
            var @event = CreateEventTitle(string.Empty);

            var eventRepository = new EventRepository() as IEventRepository;
            eventRepository.Add(@event.Id, @event);

            var bookingService = CreateBookingServiceWithEventRepository(eventRepository);

            Assert.True(eventRepository.TryRemove(@event.Id, out _));
            Assert.False(eventRepository.ContainsKey(@event.Id));

            var booking = bookingService.CreateBooking(@event.Id);
            Assert.Null(booking);
        }

        [Fact(DisplayName = "получение брони по несуществующему Id")]
        public async Task GetNotExitedBookingTest()
        {
            var bookingService = CreateBookingService();
            var booking = bookingService.GetBookingById(Guid.NewGuid());
            Assert.Null(booking);
        }
    }
}
