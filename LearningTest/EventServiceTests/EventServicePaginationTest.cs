using LearningWebApi.Entities;
using LearningWebApi.Services.EventService;
using static LearningTest.Factories.ServiceFactory;

namespace LearningTest.EventServiceTests
{
    public class EventServicePaginationTest
    {
        private IEnumerable<Event> _randomData;
        private IEventService _eventService;

        public EventServicePaginationTest()
        {
            var rnd = new Random();

            DateTime GetRandomDate()
            {
                return DateTime.Now.AddDays(rnd.Next(1, 30))
                    .AddHours(rnd.Next(1, 24))
                    .AddMinutes(rnd.Next(1, 60))
                    .AddSeconds(rnd.Next(1, 60));
            }

            _randomData = Enumerable.Range(1, 100)
                .Select(i => new Event
                {
                    Id = Guid.NewGuid(),
                    Title = $"Event {i}",
                    StartAt = GetRandomDate(),
                    EndAt = GetRandomDate(),
                    Description = $"Random description {i}"
                })
                .ToList();
            _eventService = CreateEventService(_randomData);
        }

        [Theory(DisplayName = "пагинация событий номер страницы")]
        [InlineData(1)]
        [InlineData(20)]
        public void PaginationPageNumberTest(int pageNumber)
        {
            var pageSize = 10;
            var actual = _eventService.GetEvents()
                .Pagination(pageNumber, pageSize)
                .Count();

            Assert.True(actual <= pageSize);
            Assert.True(actual >= 0);
        }

        [Theory(DisplayName = "пагинация событий некорректный номер страницы")]
        [InlineData(-1)]
        [InlineData(0)]
        public void PaginationPageNumberFailTest(int pageNumber)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _eventService.GetEvents()
                    .Pagination(pageNumber, pageSize: 10);
            });
        }

        [Theory(DisplayName = "пагинация событий количество событий на странице")]
        [InlineData(1)]
        [InlineData(20)]
        public void PaginationPageSizeTest(int pageSize)
        {
            var actual = _eventService.GetEvents()
                .Pagination(page: 1, pageSize)
                .Count();

            Assert.True(actual <= pageSize);
            Assert.True(actual >= 0);
        }

        [Theory(DisplayName = "пагинация событий некорректное количество событий на странице")]
        [InlineData(-1)]
        [InlineData(0)]
        public void PaginationPageSizeFailTest(int pageSize)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                _eventService.GetEvents()
                .Pagination(page: 1, pageSize);
            });
        }

        [Theory(DisplayName = "пагинация - проверка состава элементов (ожидаемый диапазон)")]
        [InlineData(1, 10)]
        [InlineData(2, 5)]
        public void PaginationOrderElementsTest(int pageNumber, int pageSize)
        {
            var expected = _randomData.OrderBy(c => c.StartAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var actual = _eventService.GetEvents()
                .Pagination(pageNumber, pageSize);

            Assert.Equal(expected, actual);
        }
    }
}
