using Learning.IntegrationTests.Helpers;
using LearningWebApi.Entities;
using Microsoft.EntityFrameworkCore;
using static Learning.IntegrationTests.Helpers.EntityFactory;

namespace Learning.IntegrationTests.RepositoriesTests
{
    /// <summary>
    /// Все методы репозитория событий
    /// </summary>
    [Collection("SequentialTests")]
    public class EventRepositoryCrudTests : ADockerDB
    {
        [Fact(DisplayName = "01. Создать событие")]
        public async Task CreateAsyncTest()
        {
            // Arrange
            await using var context = CreateContext();

            var repository = CreateEventRepository(context);
            var eventId = Guid.NewGuid();
            var @event = CreateEvent(eventId);

            // Act
            await repository.CreateAsync(@event);

            // Assert
            await using var verifyContext = CreateContext();
            var saved = await verifyContext.Events
                .FirstOrDefaultAsync(b => b.Id == eventId);

            Assert.NotNull(saved);
            Assert.Equal(@event.Id, saved.Id);
            Assert.Equal(@event.Title, saved.Title);
            Assert.Equal(@event.StartAt, saved.StartAt, TimeSpan.FromMilliseconds(1));
            Assert.Equal(@event.EndAt, saved.EndAt, TimeSpan.FromMilliseconds(1));
            Assert.Equal(@event.Description, saved.Description);
            Assert.Equal(@event.TotalSeats, saved.TotalSeats);
            Assert.Equal(@event.AvailableSeats, saved.AvailableSeats);
        }

        [Fact(DisplayName = "02. Получить событие")]
        public async Task GetAsyncTest()
        {
            // Arrange
            await using var context = CreateContext();
            var eventId = Guid.NewGuid();
            var expected = CreateEvent(eventId);
            context.Events.Add(expected);
            await context.SaveChangesAsync();

            // Act
            await using var getContext = CreateContext();
            var repository = CreateEventRepository(getContext);
            var actual = await repository.GetAsync(eventId);

            Assert.NotNull(actual);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.StartAt, actual.StartAt, TimeSpan.FromMilliseconds(1));
            Assert.Equal(expected.EndAt, actual.EndAt, TimeSpan.FromMilliseconds(1));
            Assert.Equal(expected.Description, actual.Description);
            Assert.Equal(expected.TotalSeats, actual.TotalSeats);
            Assert.Equal(expected.AvailableSeats, actual.AvailableSeats);
        }

        [Fact(DisplayName = "03. Обновить событие")]
        public async Task TryUpdateAsyncTest()
        {
            // Arrange
            await using var context = CreateContext();
            var eventId = Guid.NewGuid();
            var @event1 = CreateEvent(eventId);
            context.Events.Add(@event1);
            await context.SaveChangesAsync();  

            var repository = CreateEventRepository(context);
            var @event2 = new Event
            {
                Id = eventId,
                Title = "Событие 2",
                StartAt = DateTime.Now.AddHours(1),
                EndAt = DateTime.Now.AddHours(2),
                Description = "Описание",
                TotalSeats = 6,
                AvailableSeats = 4,
            };

            // Act
            var count = await repository.TryUpdateAsync(@event2);

            // Assert
            Assert.Equal(1, count);
            await using var verifyContext = CreateContext();
            var updatedCollection = await verifyContext.Events
                .Where(b => b.Id == eventId)
                .ToListAsync();

            Assert.True(updatedCollection.Count() == 1);
            var updated = updatedCollection.FirstOrDefault();
            Assert.NotNull(updated);
            Assert.Equal(@event2.Id, updated.Id);
            Assert.Equal(@event2.Title, updated.Title);
            Assert.Equal(@event2.StartAt, updated.StartAt, TimeSpan.FromMilliseconds(1));
            Assert.Equal(@event2.EndAt, updated.EndAt, TimeSpan.FromMilliseconds(1));
            Assert.Equal(@event2.Description, updated.Description);
            Assert.Equal(@event2.TotalSeats, updated.TotalSeats);
            Assert.Equal(@event2.AvailableSeats, updated.AvailableSeats);
        }

        [Fact(DisplayName = "04. Удалить событие")]
        public async Task TryRemoveAsyncTest()
        {
            // Arrange
            await using var context = CreateContext();
            var eventId = Guid.NewGuid();
            var @event = CreateEvent(eventId);
            context.Events.Add(@event);
            await context.SaveChangesAsync();
            var repository = CreateEventRepository(context);

            // Act
            var count = await repository.TryRemoveAsync(eventId);
            Assert.Equal(1, count);

            // Assert
            await using var verifyContext = CreateContext();
            var deletedCount = verifyContext.Events.Count(b => b.Id == eventId);
            Assert.Equal(0, deletedCount);
        }

        [Fact(DisplayName = "05. Обновление контекста без сохранения")]
        public async Task TryUpdateContextAsyncTest()
        {
            // Arrange
            await using var context = CreateContext();
            var eventId = Guid.NewGuid();
            var @event1 = CreateEvent(eventId);
            context.Events.Add(@event1);
            await context.SaveChangesAsync();

            var repository = CreateEventRepository(context);
            var @event2 = new Event
            {
                Id = eventId,
                Title = "Событие 2",
                StartAt = DateTime.Now.AddHours(4),
                EndAt = DateTime.Now.AddHours(5),
                Description = "Описание",
                TotalSeats = 6,
                AvailableSeats = 4,
            };

            // Act (кладём обновлённое событие в кеш контекста)
            Assert.True(await repository.TryUpdateContextAsync(@event2));

            // Assert (объект в кеше другого контекста ещё не в базе)
            await using var verifyContext = CreateContext();
            var notUpdated = await verifyContext.Events.SingleAsync(b => b.Id == eventId);
            Assert.NotEqual(@event2.Title, notUpdated.Title);
            Assert.NotEqual(@event2.StartAt, notUpdated.StartAt);
            Assert.NotEqual(@event2.EndAt, notUpdated.EndAt);
            Assert.NotEqual(@event2.Description, notUpdated.Description);
            Assert.NotEqual(@event2.TotalSeats, notUpdated.TotalSeats);
            Assert.NotEqual(@event2.AvailableSeats, notUpdated.AvailableSeats);

            // Act (сохраняем в базу)
            await context.SaveChangesAsync();
            verifyContext.ChangeTracker.Clear();

            // Assert (теперь объект действительно сохранён - проверяем)
            var updatedCollection = await verifyContext.Events
                .Where(b => b.Id == eventId)
                .ToListAsync();
            Assert.Single(updatedCollection);

            var updated = updatedCollection.FirstOrDefault();
            Assert.NotNull(updated);
            Assert.Equal(@event2.Id, updated.Id);
            Assert.Equal(@event2.Title, updated.Title);
            Assert.Equal(@event2.StartAt, updated.StartAt, TimeSpan.FromMilliseconds(1));
            Assert.Equal(@event2.EndAt, updated.EndAt, TimeSpan.FromMilliseconds(1));
            Assert.Equal(@event2.Description, updated.Description);
            Assert.Equal(@event2.TotalSeats, updated.TotalSeats);
            Assert.Equal(@event2.AvailableSeats, updated.AvailableSeats);
        }
    }
}
