using LearningTest.Helpers;
using LearningWebApi.Entities;
using LearningWebApi.Services.EventService;
using static LearningTest.Helpers.EntityFactory;

namespace LearningTest.EventServiceTests
{
    [Trait("Category", "Unit")]
    public class EventServiceFilterTest : AServiceCollection
    {
        [Fact(DisplayName = "01. Фильтрация по названию")]
        public async Task FilterByTitleTest()
        {
            var toCreateTitle = CreateEventTitle("ToCreateTitle");
            var toUpdateTitle = CreateEventTitle("ToUpdateTitle");
            var toDeleteTitle = CreateEventTitle("ToDeleteTitle");
            var justData = CreateEventTitle("justData");

            IEnumerable<Event> all = [toCreateTitle, toUpdateTitle, toDeleteTitle, justData];
            var eventService = GetInitializedService<IEventService, Event>(all);

            async Task<(IEnumerable<Event>, IEnumerable<Event>)> FilterByTitle(IEnumerable<Event> collection, string? title = null)
            {
                return (collection.OrderBy(a => a.Id), (await eventService.GetEventsAsync(page: 1, pageSize: all.Count(), title: title)).OrderBy(a => a.Id));
            }

            var (expected, actual) = await FilterByTitle(all);
            Assert.Equal(expected, actual);

            (expected, actual) = await FilterByTitle([toCreateTitle, toUpdateTitle, toDeleteTitle], "To");
            Assert.Equal(expected, actual);

            (expected, actual) = await FilterByTitle([toCreateTitle, toUpdateTitle, toDeleteTitle], "title");
            Assert.Equal(expected, actual);

            (expected, actual) = await FilterByTitle([toCreateTitle], "create");
            Assert.Equal(expected, actual);

            (expected, actual) = await FilterByTitle([toUpdateTitle], "update");
            Assert.Equal(expected, actual);

            (expected, actual) = await FilterByTitle([toCreateTitle, toUpdateTitle], "ate");
            Assert.Equal(expected, actual);
        }

        [Fact(DisplayName = "02. Фильтрация по дате startDate")]
        public async Task FilterByFromTest()
        {
            var minOrDefault = CreateEventStartAt(default);
            var less = CreateEventStartAt(DateTime.Now.AddHours(-1));
            var now = CreateEventStartAt(DateTime.Now);
            var more = CreateEventStartAt(DateTime.Now.AddHours(1));
            var max = CreateEventStartAt(DateTime.MaxValue);

            IEnumerable<Event> all = [minOrDefault, less, now, more, max];
            var eventService = GetInitializedService<IEventService, Event>(all);

            async Task<(IEnumerable<Event>, IEnumerable<Event>)> FilterByFrom(IEnumerable<Event> collection, DateTime? from)
            {
                return (collection.OrderBy(a => a.StartAt), (await eventService.GetEventsAsync(page: 1, pageSize: all.Count(), from: from)).OrderBy(a => a.StartAt));
            }

            var (expected, actual) = await FilterByFrom(all, null);
            Assert.Equal(expected, actual);

            (expected, actual) = await FilterByFrom(all, default);
            Assert.Equal(expected, actual);

            (expected, actual) = await FilterByFrom(all, DateTime.MinValue);
            Assert.Equal(expected, actual);

            (expected, actual) = await FilterByFrom([less, now, more, max], less.StartAt);
            Assert.Equal(expected, actual);

            (expected, actual) = await FilterByFrom([now, more, max], now.StartAt);
            Assert.Equal(expected, actual);

            (expected, actual) = await FilterByFrom([more, max], more.StartAt);
            Assert.Equal(expected, actual);

            (expected, actual) = await FilterByFrom([max], max.StartAt);
            Assert.Equal(expected, actual);
        }

        [Fact(DisplayName = "03. Фильтрация по дате endDate")]
        public async Task FilterByToTest()
        {
            var minOrDefault = CreateEventEndAt(default);
            var less = CreateEventEndAt(DateTime.Now.AddHours(-1));
            var now = CreateEventEndAt(DateTime.Now);
            var more = CreateEventEndAt(DateTime.Now.AddHours(1));
            var max = CreateEventEndAt(DateTime.MaxValue);

            IEnumerable<Event> all = [minOrDefault, less, now, more, max];
            var eventService = GetInitializedService<IEventService, Event>(all);

            async Task<(IEnumerable<Event>, IEnumerable<Event>)> FilterByTo(IEnumerable<Event> collection, DateTime? to)
            {
                return (collection.OrderBy(a => a.EndAt), (await eventService.GetEventsAsync(page: 1, pageSize: all.Count(), to: to)).OrderBy(a => a.EndAt));
            }

            var (expected, actual) = await FilterByTo(all, null);
            Assert.Equal(expected, actual);

            (expected, actual) = await FilterByTo(all, default);
            Assert.Equal(expected, actual);

            (expected, actual) = await FilterByTo([minOrDefault], DateTime.MinValue);
            // Обнаружилась особенность InMemoryDatabase с проверкой минимальных значений в дереве выражений where
            // (две идентичных даты вплоть до наносекунд не срабатывают по условию data.Where(a => a.EndAt <= dateTime.Value))
            //Assert.Equal(expected, actual);

            (expected, actual) = await FilterByTo([minOrDefault, less], less.EndAt);
            Assert.Equal(expected, actual);

            (expected, actual) = await FilterByTo([minOrDefault, less, now], now.EndAt);
            Assert.Equal(expected, actual);

            (expected, actual) = await FilterByTo([minOrDefault, less, now, more], more.EndAt);
            Assert.Equal(expected, actual);

            (expected, actual) = await FilterByTo(all, max.EndAt);
            Assert.Equal(expected, actual);
        }

        [Theory(DisplayName = "04. Комбинированная фильтрация")]
        [InlineData("one", null, null, 3)]
        [InlineData("two", null, null, 3)]
        [InlineData("three", null, null, 3)]
        [InlineData("past", null, null, 3)]
        [InlineData("skip", null, null, 3)]
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
            var now = DateTime.Now;
            var pastone = CreateEvent("pastone", now.AddHours(-1), now);
            var pasttwo = CreateEvent("pasttwo", now.AddHours(-2), now);
            var pastthree = CreateEvent("pastthree", now.AddHours(-3), now);
            var one = CreateEvent("one", now, now.AddHours(1));
            var two = CreateEvent("two", now, now.AddHours(2));
            var three = CreateEvent("three", now, now.AddHours(3));
            var skipone = CreateEvent("skipone", now.AddHours(1), now.AddHours(2));
            var skiptwo = CreateEvent("skiptwo", now.AddHours(2), now.AddHours(3));
            var skipthree = CreateEvent("skipthree", now.AddHours(3), now.AddHours(4));
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
