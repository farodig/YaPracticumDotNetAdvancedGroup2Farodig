using LearningWebApi.Entities;
using LearningWebApi.Exceptions;
using static LearningTest.Factories.EntityFactory;
using static LearningTest.Factories.MockRepositoryFactory;
using static LearningTest.Factories.RepositoryFactory;
using static LearningTest.Factories.ServiceFactory;

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
            using var bookingProcessor = await CreateBookingProcessor(bookingRepository, eventRepository);

            var booking = bookingService.CreateBooking(@event.Id);

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
            using var bookingProcessor = await CreateBookingProcessor(bookingRepository, eventRepository);

            var booking = bookingService.CreateBooking(@event.Id);
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


        [Theory(DisplayName = "Тест на защиту от овербукинга")]
        // Дано: событие на 5 мест, 20 конкурентных запросов
        // Ожидается: ровно 5 успешных броней, 15 — NoAvailableSeatsException
        [InlineData(5, 20, 5, 15)]
        public async Task OverbookingProtectionTest(int available, int conccurrent,
            int expectedConfirmed, int expectedException)
        {
            var @event = CreateEventAvailableSeats(available);
            var eventRepository = MockEventRepository(@event);
            var bookingService = CreateBookingService(eventRepository);

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
            var bookingService = CreateBookingService(eventRepository);

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
