using LearningWebApi.Services.EventService;
using static LearningTest.Factories.ServiceFactory;
using static LearningTest.Factories.EntityFactory;
using LearningWebApi.Entities;

namespace LearningTest.EventServiceTests
{
    public class EventServiceFilterTest
    {
        [Fact(DisplayName = "фильтрация по названию")]
        public void FilterByTitleTest()
        {
            var toCreateTitle = CreateEventTitle("ToCreateTitle");
            var toUpdateTitle = CreateEventTitle("ToUpdateTitle");
            var toDeleteTitle = CreateEventTitle("ToDeleteTitle");

            IEnumerable<Event> data = [toCreateTitle, toUpdateTitle, toDeleteTitle];
            var eventService = CreateEventService(data);

            IEnumerable<Event> expected = data.OrderBy(a => a.Id);
            var actual = eventService.GetEvents().FilterByTitle("To").OrderBy(a => a.Id).ToArray();
            Assert.Equal(expected, actual);

            actual = eventService.GetEvents().FilterByTitle("title").OrderBy(a => a.Id).ToArray();
            Assert.Equal(expected, actual);

            expected = [toCreateTitle];
            actual = eventService.GetEvents().FilterByTitle("create").ToArray();
            Assert.Equal(expected, actual);

            expected = [toUpdateTitle];
            actual = eventService.GetEvents().FilterByTitle("update").ToArray();
            Assert.Equal(expected, actual);

            expected = new[] { toCreateTitle, toUpdateTitle }.OrderBy(a => a.Id).ToArray();
            actual = eventService.GetEvents().FilterByTitle("ate").OrderBy(a => a.Id).ToArray();
            Assert.Equal(expected, actual);
        }

        [Fact(DisplayName = "фильтрация по дате startDate")]
        public void FilterByFromTest()
        {
            var minOrDefault = CreateEventStartAt(default);
            var less = CreateEventStartAt(DateTime.Now.AddHours(-1));
            var now = CreateEventStartAt(DateTime.Now);
            var more = CreateEventStartAt(DateTime.Now.AddHours(1));
            var max = CreateEventStartAt(DateTime.MaxValue);

            IEnumerable<Event> all = [minOrDefault, less, now, more, max];
            var eventService = CreateEventService(all);

            var expected = all.OrderBy(a => a.StartAt);
            var actual = eventService.GetEvents().FilterByFrom(null).OrderBy(a => a.StartAt);
            Assert.Equal(expected, actual);

            expected = all.OrderBy(a => a.StartAt);
            actual = eventService.GetEvents().FilterByFrom(default).OrderBy(a => a.StartAt);
            Assert.Equal(expected, actual);

            expected = all.OrderBy(a => a.StartAt);
            actual = eventService.GetEvents().FilterByFrom(DateTime.MinValue).OrderBy(a => a.StartAt);
            Assert.Equal(expected, actual);

            expected = new[] { less, now, more, max }.OrderBy(a => a.StartAt);
            actual = eventService.GetEvents().FilterByFrom(less.StartAt).OrderBy(a => a.StartAt);
            Assert.Equal(expected, actual);

            expected = new[] { now, more, max }.OrderBy(a => a.StartAt);
            actual = eventService.GetEvents().FilterByFrom(now.StartAt).OrderBy(a => a.StartAt);
            Assert.Equal(expected, actual);

            expected = new[] { more, max }.OrderBy(a => a.StartAt);
            actual = eventService.GetEvents().FilterByFrom(more.StartAt).OrderBy(a => a.StartAt);
            Assert.Equal(expected, actual);

            expected = new[] { max }.OrderBy(a => a.StartAt);
            actual = eventService.GetEvents().FilterByFrom(max.StartAt).OrderBy(a => a.StartAt);
            Assert.Equal(expected, actual);
        }

        [Fact(DisplayName = "фильтрация по дате endDate")]
        public void FilterByToTest()
        {
            var minOrDefault = CreateEventEndAt(default);
            var less = CreateEventEndAt(DateTime.Now.AddHours(-1));
            var now = CreateEventEndAt(DateTime.Now);
            var more = CreateEventEndAt(DateTime.Now.AddHours(1));
            var max = CreateEventEndAt(DateTime.MaxValue);

            IEnumerable<Event> all = [minOrDefault, less, now, more, max];
            var eventService = CreateEventService(all);

            var expected = all.OrderBy(a => a.EndAt);
            var actual = eventService.GetEvents().FilterByTo(null).OrderBy(a => a.EndAt);
            Assert.Equal(expected, actual);

            expected = all.OrderBy(a => a.EndAt);
            actual = eventService.GetEvents().FilterByTo(default).OrderBy(a => a.EndAt);
            Assert.Equal(expected, actual);

            expected = new[] { minOrDefault }.OrderBy(a => a.EndAt);
            actual = eventService.GetEvents().FilterByTo(DateTime.MinValue).OrderBy(a => a.EndAt);
            Assert.Equal(expected, actual);

            expected = new[] { minOrDefault, less }.OrderBy(a => a.EndAt);
            actual = eventService.GetEvents().FilterByTo(less.EndAt).OrderBy(a => a.EndAt);
            Assert.Equal(expected, actual);

            expected = new[] { minOrDefault, less, now }.OrderBy(a => a.EndAt);
            actual = eventService.GetEvents().FilterByTo(now.EndAt).OrderBy(a => a.EndAt);
            Assert.Equal(expected, actual);

            expected = new[] { minOrDefault, less, now, more }.OrderBy(a => a.EndAt);
            actual = eventService.GetEvents().FilterByTo(more.EndAt).OrderBy(a => a.EndAt);
            Assert.Equal(expected, actual);

            expected = all.OrderBy(a => a.EndAt);
            actual = eventService.GetEvents().FilterByTo(max.EndAt).OrderBy(a => a.EndAt);
            Assert.Equal(expected, actual);
        }

        [Theory(DisplayName = "комбинированная фильтрация")]
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
        public void CombinationFilterTest(string? title, int? diffStartAt, int? diffEndAt, int expected)
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
            var eventService = CreateEventService(all);

            var actual = eventService.GetEvents()
                .FilterByTitle(title)
                .FilterByFrom(diffStartAt.HasValue ? now.AddHours(diffStartAt.Value) : null)
                .FilterByTo(diffEndAt.HasValue ? now.AddHours(diffEndAt.Value) : null)
                .Count();

            Assert.Equal(expected, actual);
        }
    }
}
