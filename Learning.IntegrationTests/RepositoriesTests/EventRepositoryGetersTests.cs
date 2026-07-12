using Domain.Entities;
using Learning.IntegrationTests.Helpers;
using static Learning.IntegrationTests.Helpers.EntityFactory;

namespace Learning.IntegrationTests.RepositoriesTests
{
    /// <summary>
    /// Все вариации фильтров репозитория событий
    /// </summary>
    [Collection("SequentialTests")]
    public class EventRepositoryGetersTests : ADockerDB
    {
        [Fact(DisplayName = "01. Фильтрация событий по заголовку")]
        public async Task FilterGetEventsAsync()
        {
            // Arrange
            var titleA = CreateEvent(title: "titleA");
            var titleB = CreateEvent(title: "titleB");
            var titleC = CreateEvent(title: "titleC");
            var toCreateTitle = CreateEvent(title: "ToCreateTitle");
            var toUpdateTitle = CreateEvent(title: "ToUpdateTitle");
            var toDeleteTitle = CreateEvent(title: "ToDeleteTitle");
            var justData = CreateEvent(title: "justData");

            IEnumerable<Event> all = [titleA, titleB, titleC, toCreateTitle, toUpdateTitle, toDeleteTitle, justData];
            await using var context = CreateContext();
            await context.Events.AddRangeAsync(all);
            await context.SaveChangesAsync();

            await using var verifyContext = CreateContext();
            var repository = CreateEventRepository(verifyContext);

            // Act
            async Task<(IEnumerable<string>, IEnumerable<string>)> FilterByTitle(IEnumerable<Event> collection, string? title = null)
            {
                return (collection.OrderBy(a => a.Id).Select(a => a.Title),
                    (await repository.GetEventsAsync(page: 1, pageSize: all.Count(), title: title)).OrderBy(a => a.Id).Select(a => a.Title));
            }

            // Assert
            var (expected, actual) = await FilterByTitle(all);
            Assert.Equal(expected, actual);

            (expected, actual) = await FilterByTitle([titleA, titleB, titleC, toCreateTitle, toUpdateTitle, toDeleteTitle], "title");
            Assert.Equal(expected, actual);

            (expected, actual) = await FilterByTitle([titleA, toCreateTitle], "eA");
            Assert.Equal(expected, actual);

            (expected, actual) = await FilterByTitle([toCreateTitle, toUpdateTitle], "ate");
            Assert.Equal(expected, actual);

            (expected, actual) = await FilterByTitle([], "Ba");
            Assert.Equal(expected, actual);
        }


        [Fact(DisplayName = "02. Фильтрация по дате startDate")]
        public async Task FilterByFromTest()
        {
            // Arrange
            var now = CreateEvent(startAt: DateTime.Now.AddMinutes(1));
            var more = CreateEvent(startAt: DateTime.Now.AddHours(1));
            var max = CreateEvent(startAt: DateTime.Now.AddYears(1000), endAt: DateTime.Now.AddYears(1001));

            IEnumerable<Event> all = [now, more, max];
            await using var context = CreateContext();
            await context.Events.AddRangeAsync(all);
            await context.SaveChangesAsync();

            await using var verifyContext = CreateContext();
            var repository = CreateEventRepository(verifyContext);

            // Act
            async Task<(IEnumerable<DateTime>, IEnumerable<DateTime>)> FilterByFrom(IEnumerable<Event> collection, DateTime? from)
            {
                return (collection.OrderBy(a => a.StartAt).Select(a => a.StartAt),
                    (await repository.GetEventsAsync(page: 1, pageSize: all.Count(), from: from)).OrderBy(a => a.StartAt).Select(a => a.StartAt));
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

            // Assert
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

        [Fact(DisplayName = "03. Фильтрация по дате endDate")]
        public async Task FilterByToTest()
        {
            var min = CreateEvent(endAt: DateTime.Now.AddHours(2));
            var more = CreateEvent(endAt: DateTime.Now.AddHours(3));
            var max = CreateEvent(endAt: DateTime.Now.AddYears(1000));

            IEnumerable<Event> all = [min, more, max];
            await using var context = CreateContext();
            await context.Events.AddRangeAsync(all);
            await context.SaveChangesAsync();

            await using var verifyContext = CreateContext();
            var repository = CreateEventRepository(verifyContext);

            // Act
            async Task<(IEnumerable<DateTime>, IEnumerable<DateTime>)> FilterByTo(IEnumerable<Event> collection, DateTime? to)
            {
                return (collection.OrderBy(a => a.EndAt).Select(a => a.EndAt),
                    (await repository.GetEventsAsync(page: 1, pageSize: all.Count(), to: to)).OrderBy(a => a.EndAt).Select(a => a.EndAt));
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

            var (expected, actual) = await FilterByTo(all, null);
            ZipEqual(expected, actual);

            (expected, actual) = await FilterByTo(all, default);
            ZipEqual(expected, actual);

            (expected, actual) = await FilterByTo([min], min.EndAt);
            ZipEqual(expected, actual);

            (expected, actual) = await FilterByTo([min, more], more.EndAt);
            ZipEqual(expected, actual);

            (expected, actual) = await FilterByTo(all, max.EndAt);
            ZipEqual(expected, actual);
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
            // Arrange
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
            await using var context = CreateContext();
            await context.Events.AddRangeAsync(all);
            await context.SaveChangesAsync();

            await using var verifyContext = CreateContext();
            var repository = CreateEventRepository(verifyContext);

            var actual = (await repository.GetEventsAsync(page: 1, pageSize: all.Count(),
                title: title,
                from: diffStartAt.HasValue ? now.AddHours(diffStartAt.Value) : null,
                to: diffEndAt.HasValue ? now.AddHours(diffEndAt.Value) : null)).Count();

            Assert.Equal(expected, actual);
        }
    }
}
