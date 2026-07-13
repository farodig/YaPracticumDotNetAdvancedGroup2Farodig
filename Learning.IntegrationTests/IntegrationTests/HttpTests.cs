using Learning.IntegrationTests.Helpers;
using Presentation.Models.Requests;
using Presentation.Models.Responses;
using System.Net;

namespace Learning.UnitTests.IntegrationTests
{
    [Trait("Category", "Integration")]
    [Collection("SequentialTests")]
    public class HttpTests(IntegrationTestFactory factory) : AHttpClient(factory)
    {
        [Fact(DisplayName = "01. Создание объекта")]
        public async Task CreateEventTest()
        {
            var request = new CreateEventRequest
            {
                Title = "CreateEventTest",
                Description = "01. Создание объекта",
                StartAt = DateTime.Now.AddHours(1),
                EndAt = DateTime.Now.AddHours(1).AddSeconds(1),
                TotalSeats = 3,
            };
            var response = await PostAsync<EventResponse>("/events", request);

            Assert.NotNull(response);
            Assert.Equal(request.Title, response.Title);
            Assert.Equal(request.Description, response.Description);
            Assert.Equal(request.StartAt, response.StartAt);
            Assert.Equal(request.EndAt, response.EndAt);
            Assert.Equal(request.TotalSeats, response.TotalSeats);

            Assert.Equal(response.TotalSeats, response.AvailableSeats);
        }

        [Fact(DisplayName = "02. Создание брони")]
        public async Task CreateBookingTest()
        {
            var toCreateEvent = new CreateEventRequest
            {
                Title = "CreateBookingTest",
                Description = "02. Создание брони",
                StartAt = DateTime.Now.AddHours(1),
                EndAt = DateTime.Now.AddHours(1).AddSeconds(1),
                TotalSeats = 3,
            };
            var createdEvent = await PostAsync<EventResponse>("/events", toCreateEvent);
            Assert.NotNull(createdEvent);

            var createdBooking = await PostAsync<BookingResponse>($"/events/{createdEvent.Id}/book");
            Assert.NotNull(createdBooking);
            Assert.Equal(createdEvent.Id, createdBooking.EventId);
            Assert.Equal(BookingStatus.Pending, createdBooking.Status);
            Assert.Null(createdBooking.ProcessedAt);
        }

        [Fact(DisplayName = "03. Нельзя создать больше бронирований чем доступно в событии")]
        public async Task CreateOverflowBookingTest()
        {
            var toCreateEvent = new CreateEventRequest
            {
                Title = "CreateOverflowBookingTest",
                Description = "03. Нельзя создать больше бронирований чем доступно в событии",
                StartAt = DateTime.Now.AddHours(1),
                EndAt = DateTime.Now.AddHours(1).AddSeconds(1),
                TotalSeats = 1,
            };
            var createdEvent = await PostAsync<EventResponse>("/events", toCreateEvent);
            Assert.NotNull(createdEvent);

            var createdBooking = await PostAsync<BookingResponse>($"/events/{createdEvent.Id}/book");
            Assert.NotNull(createdBooking);
            Assert.Equal(BookingStatus.Pending, createdBooking.Status);

            var exception = await Assert.ThrowsAsync<HttpRequestException>(async () => await PostAsync<BookingResponse>($"/events/{createdEvent.Id}/book"));
            Assert.Equal(HttpStatusCode.Conflict, exception.StatusCode);
        }

        [Fact(DisplayName = "04. Проверка успешной обработки бронирования")]
        public async Task ProcessBookingTest()
        {
            var toCreateEvent = new CreateEventRequest
            {
                Title = "ProcessBookingTest",
                Description = "04. Проверка успешной обработки бронирования",
                StartAt = DateTime.Now.AddHours(1),
                EndAt = DateTime.Now.AddHours(1).AddSeconds(1),
                TotalSeats = 1,
            };
            var createdEvent = await PostAsync<EventResponse>("/events", toCreateEvent);
            Assert.NotNull(createdEvent);

            var createdBooking = await PostAsync<BookingResponse>($"/events/{createdEvent.Id}/book");
            Assert.NotNull(createdBooking);
            Assert.Equal(BookingStatus.Pending, createdBooking.Status);

            await Task.Delay(TimeSpan.FromSeconds(5));

            var processedBooking = await GetAsync<BookingResponse>($"/booking/{createdBooking.Id}");
            Assert.NotNull(processedBooking);
            Assert.Equal(BookingStatus.Confirmed, processedBooking.Status);
        }
    }
}
