using Domain.Entities;
using Learning.IntegrationTests.Helpers;
using static Learning.IntegrationTests.Helpers.EntityFactory;

namespace Learning.IntegrationTests.RepositoriesTests
{
    /// <summary>
    /// Пагинация в методе репозитория получения списка событий
    /// </summary>
    [Collection("SequentialTests")]
    public class EventRepositoryPaginationTests : ADockerDB
    {
        private async Task<IEnumerable<Event>> InitializeRandomDataAsync(int count = 100)
        {
            var data = Enumerable.Range(1, count)
                .Select(i => CreateEvent(
                    title: $"Event {i}",
                    description: $"Random description {i}"));

            await using var context = CreateContext();
            await context.Events.AddRangeAsync(data);
            await context.SaveChangesAsync();

            return data;
        }

        [Theory(DisplayName = "01. Пагинация событий номер страницы")]
        [InlineData(1)]
        [InlineData(20)]
        public async Task PaginationPageNumberTest(int pageNumber)
        {
            // Arrange
            await InitializeRandomDataAsync();
            var pageSize = 10;
            await using var verifyContext = CreateContext();
            var repository = CreateEventRepository(verifyContext);

            // Act
            var actual = (await repository.GetEventsAsync(pageNumber, pageSize))
                .Count();

            // Assert
            Assert.True(actual <= pageSize);
            Assert.True(actual >= 0);
        }

        [Theory(DisplayName = "02. Пагинация событий некорректный номер страницы")]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task PaginationPageNumberFailTest(int pageNumber)
        {
            // Arrange
            await InitializeRandomDataAsync();
            await using var verifyContext = CreateContext();
            var repository = CreateEventRepository(verifyContext);

            // Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            {
                // Act
                await repository.GetEventsAsync(pageNumber, pageSize: 10);
            });
        }

        [Theory(DisplayName = "03. Пагинация событий количество событий на странице")]
        [InlineData(1)]
        [InlineData(20)]
        public async Task PaginationPageSizeTest(int pageSize)
        {
            // Arrange
            await InitializeRandomDataAsync();
            await using var verifyContext = CreateContext();
            var repository = CreateEventRepository(verifyContext);

            // Act
            var actual = (await repository.GetEventsAsync(page: 1, pageSize))
                .Count();

            // Assert
            Assert.True(actual <= pageSize);
            Assert.True(actual >= 0);
        }

        [Theory(DisplayName = "04. Пагинация событий некорректное количество событий на странице")]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task PaginationPageSizeFailTest(int pageSize)
        {
            // Arrange
            await InitializeRandomDataAsync();
            await using var verifyContext = CreateContext();
            var repository = CreateEventRepository(verifyContext);

            // Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            {
                // Act
                await repository.GetEventsAsync(page: 1, pageSize);
            });
        }

        [Theory(DisplayName = "05. Пагинация - проверка состава элементов (ожидаемый диапазон)")]
        [InlineData(1, 10)]
        [InlineData(2, 5)]
        public async Task PaginationOrderElementsTest(int pageNumber, int pageSize)
        {
            // Arrange
            var data = await InitializeRandomDataAsync(10);
            var expected = data.OrderBy(c => c.StartAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            await using var verifyContext = CreateContext();
            var repository = CreateEventRepository(verifyContext);

            // Act
            var actual = await repository.GetEventsAsync(pageNumber, pageSize);

            // Assert
            Assert.Equal(pageSize, actual.Count());
        }
    }
}
