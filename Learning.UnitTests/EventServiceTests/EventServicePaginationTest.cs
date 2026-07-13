using Application.Services.EventService;
using Domain.Entities;
using Learning.UnitTests.Helpers;

namespace Learning.UnitTests.EventServiceTests
{
    [Trait("Category", "Unit")]
    public class EventServicePaginationTest : AServiceCollection
    {
        private readonly IEnumerable<Event> _randomData;
        private readonly IEventService _eventService;

        public EventServicePaginationTest() : base()
        {
            var rnd = new Random();

            DateTime GetRandomDate()
            {
                return DateTime.Now.AddDays(rnd.Next(1, 30))
                    .AddHours(rnd.Next(1, 24))
                    .AddMinutes(rnd.Next(1, 60))
                    .AddSeconds(rnd.Next(1, 60));
            }

            _randomData = [.. Enumerable.Range(1, 100)
                .Select(i => new Event
                {
                    Id = Guid.NewGuid(),
                    Title = $"Event {i}",
                    StartAt = GetRandomDate(),
                    EndAt = GetRandomDate(),
                    Description = $"Random description {i}"
                })];
            _eventService = GetInitializedService<IEventService, Event>(_randomData);
        }

        [Theory(DisplayName = "01. Пагинация событий номер страницы")]
        [InlineData(1)]
        [InlineData(20)]
        public async Task PaginationPageNumberTest(int pageNumber)
        {
            var pageSize = 10;
            var actual = (await _eventService.GetEventsAsync(pageNumber, pageSize))
                .Count();

            Assert.True(actual <= pageSize);
            Assert.True(actual >= 0);
        }

        [Theory(DisplayName = "02. Пагинация событий некорректный номер страницы")]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task PaginationPageNumberFailTest(int pageNumber)
        {
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            {
                await _eventService.GetEventsAsync(pageNumber, pageSize: 10);
            });
        }

        [Theory(DisplayName = "03. Пагинация событий количество событий на странице")]
        [InlineData(1)]
        [InlineData(20)]
        public async Task PaginationPageSizeTest(int pageSize)
        {
            var actual = (await _eventService.GetEventsAsync(page: 1, pageSize))
                .Count();

            Assert.True(actual <= pageSize);
            Assert.True(actual >= 0);
        }

        [Theory(DisplayName = "04. Пагинация событий некорректное количество событий на странице")]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task PaginationPageSizeFailTest(int pageSize)
        {
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            {
                await _eventService.GetEventsAsync(page: 1, pageSize);
            });
        }

        [Theory(DisplayName = "05. Пагинация - проверка состава элементов (ожидаемый диапазон)")]
        [InlineData(1, 10)]
        [InlineData(2, 5)]
        public async Task PaginationOrderElementsTest(int pageNumber, int pageSize)
        {
            var expected = _randomData.OrderBy(c => c.StartAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(a => a.Id);

            var actual = (await _eventService.GetEventsAsync(pageNumber, pageSize))
                .Select(a => a.Id);

            Assert.Equal(expected, actual);
        }
    }
}
