using Domain.Entities;
using Learning.UnitTests.Helpers;
using Presentation.Services.EventService;
using static Learning.UnitTests.Helpers.EntityFactory;

namespace Learning.UnitTests.EventServiceTests
{
    [Trait("Category", "Unit")]
    public class EventServiceFilterTest : AServiceCollection
    {
        /* Примечание: тестирование фильтрации по Event.Title невозможно в контекстом БД в памяти 
         * из за метода фильтрации EF.Functions.ILike который работает только с реальной бд 
         * есть интеграционный тест проверяющий фильтрацию по Event.Title */

        [Fact(DisplayName = "01. Фильтрация по дате startDate")]
        public async Task FilterByFromTest()
        {
            var minOrDefault = CreateEvent(startAt: default);
            var now = CreateEvent(startAt: DateTime.Now.AddMinutes(1));
            var more = CreateEvent(startAt: DateTime.Now.AddHours(1));
            var max = CreateEvent(startAt: DateTime.Now.AddYears(1000), endAt: DateTime.Now.AddYears(1001));

            IEnumerable<Event> all = [now, more, max];
            var eventService = GetInitializedService<IEventService, Event>(all);

            // Act
            async Task<(IEnumerable<DateTime>, IEnumerable<DateTime>)> FilterByFrom(IEnumerable<Event> collection, DateTime? from)
            {
                return (collection.OrderBy(a => a.StartAt).Select(a => a.StartAt),
                    (await eventService.GetEventsAsync(page: 1, pageSize: all.Count(), from: from)).OrderBy(a => a.StartAt).Select(a => a.StartAt));
            }

            // Assert
            void ZipEqual(IEnumerable<DateTime> collection1, IEnumerable<DateTime> collection2)
            {
                Assert.Equal(collection1.Count(), collection2.Count());
                foreach (var (expected, actual) in collection1.Zip(collection2, (expected, actual) => (expected, actual)))
                {
                    Assert.Equal(expected, actual, TimeSpan.FromMilliseconds(1));
                }
            }

            var (expected, actual) = await FilterByFrom(all, null);
            ZipEqual(expected, actual);

            (expected, actual) = await FilterByFrom(all, default);
            ZipEqual(expected, actual);

            (expected, actual) = await FilterByFrom(all, DateTime.MinValue);
            ZipEqual(expected, actual);

            (expected, actual) = await FilterByFrom([now, more, max], now.StartAt);
            ZipEqual(expected, actual);

            (expected, actual) = await FilterByFrom([more, max], more.StartAt);
            ZipEqual(expected, actual);

            (expected, actual) = await FilterByFrom([max], max.StartAt);
            ZipEqual(expected, actual);
        }

        [Fact(DisplayName = "02. Фильтрация по дате endDate")]
        public async Task FilterByToTest()
        {
            var min = CreateEvent(endAt: DateTime.Now.AddHours(2));
            var more = CreateEvent(endAt: DateTime.Now.AddHours(3));
            var max = CreateEvent(endAt: DateTime.Now.AddYears(1000));

            IEnumerable<Event> all = [min, more, max];
            var eventService = GetInitializedService<IEventService, Event>(all);

            async Task<(IEnumerable<Event>, IEnumerable<Event>)> FilterByTo(IEnumerable<Event> collection, DateTime? to)
            {
                return (collection.OrderBy(a => a.EndAt), (await eventService.GetEventsAsync(page: 1, pageSize: all.Count(), to: to)).OrderBy(a => a.EndAt));
            }

            var (expected, actual) = await FilterByTo(all, null);
            Assert.Equal(expected, actual);

            (expected, actual) = await FilterByTo(all, default);
            Assert.Equal(expected, actual);

            (expected, actual) = await FilterByTo([min], min.EndAt);
            Assert.Equal(expected, actual);

            (expected, actual) = await FilterByTo([min, more], more.EndAt);
            Assert.Equal(expected, actual);

            (expected, actual) = await FilterByTo(all, max.EndAt);
            Assert.Equal(expected, actual);
        }

        [Theory(DisplayName = "03. Комбинированная фильтрация")]
        [InlineData(null, -3, null, 9)]
        [InlineData(null, -2, null, 8)]
        [InlineData(null, -1, null, 7)]
        [InlineData(null, 0, null, 6)]
        [InlineData(null, 1, null, 3)]
        [InlineData(null, 2, null, 2)]
        [InlineData(null, 3, null, 1)]
        [InlineData(null, null, 0, 3)]
        [InlineData(null, null, 1, 4)]
        [InlineData(null, null, 2, 6)]
        [InlineData(null, null, 3, 8)]
        [InlineData(null, null, 4, 9)]
        public async Task CombinationFilterTest(string? title, int? diffStartAt, int? diffEndAt, int expected)
        {
            var now = DateTime.Now.AddHours(10);
            var pastone = CreateEvent(title: "pastone", startAt: now.AddHours(-1), endAt: now);
            var pasttwo = CreateEvent(title: "pasttwo", startAt: now.AddHours(-2), endAt: now);
            var pastthree = CreateEvent(title: "pastthree", startAt: now.AddHours(-3), endAt: now);
            var one = CreateEvent(title: "one", startAt: now, endAt: now.AddHours(1));
            var two = CreateEvent(title: "two", startAt: now, endAt: now.AddHours(2));
            var three = CreateEvent(title: "three", startAt: now, endAt: now.AddHours(3));
            var skipone = CreateEvent(title: "skipone", startAt: now.AddHours(1), endAt: now.AddHours(2));
            var skiptwo = CreateEvent(title: "skiptwo", startAt: now.AddHours(2), endAt: now.AddHours(3));
            var skipthree = CreateEvent(title: "skipthree", startAt: now.AddHours(3), endAt: now.AddHours(4));
            IEnumerable<Event> all = [pastone, pasttwo, pastthree,
                one, two, three,
                skipone, skiptwo, skipthree];
            var eventService = GetInitializedService<IEventService, Event>(all);

            var actual = (await eventService.GetEventsAsync(page: 1, pageSize: all.Count(),
                title: title,
                from: diffStartAt.HasValue ? now.AddHours(diffStartAt.Value) : null,
                to:  diffEndAt.HasValue? now.AddHours(diffEndAt.Value) : null)).Count();

            Assert.Equal(expected, actual);
        }
    }
}
