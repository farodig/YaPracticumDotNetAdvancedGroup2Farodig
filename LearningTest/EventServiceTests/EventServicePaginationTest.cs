using LearningWebApi.Services.EventService;
using static LearningTest.Factories.EventServiceFactory;

namespace LearningTest.EventServiceTests
{
    public class EventServicePaginationTest
    {
        private IEventService _eventService;

        public EventServicePaginationTest()
        {
            var rnd = new Random();
            IEnumerable<Event> randomData = Enumerable.Range(1, 100)
                .Select(i => new Event
                {
                    Id = Guid.NewGuid(),
                    Title = $"Event {i}",
                    StartAt = DateTime.Now.AddDays(rnd.Next(1, 30)),
                    EndAt = DateTime.Now.AddDays(rnd.Next(31, 60)),
                    Description = $"Random description {i}"
                });
            _eventService = CreateEventService(randomData);
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
    }
}
