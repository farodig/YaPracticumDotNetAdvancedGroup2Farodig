using LearningTest.Helpers;
using LearningWebApi.Entities;
using LearningWebApi.Services.EventService;
using static LearningTest.Helpers.EntityFactory;

namespace LearningTest.EventServiceTests
{
    public class EventServiceCRUDTest : AServiceCollection
    {
        [Fact(DisplayName = "01. Создание события")]
        public async Task CreateEventTest()
        {
            var service = GetService<IEventService>();
            var expected = await service.CreateEventAsync("ToCreateTitle", DateTime.Now, DateTime.Now, 10, "ToCreateDescription");
            
            var actual = await service.GetEventAsync(expected.Id);
            
            Assert.NotNull(actual);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.StartAt, actual.StartAt);
            Assert.Equal(expected.EndAt, actual.EndAt);
            Assert.Equal(expected.Description, actual.Description);
            Assert.Equal(expected.TotalSeats, actual.TotalSeats);
            Assert.Equal(expected.AvailableSeats, actual.AvailableSeats);
        }

        [Fact(DisplayName = "02. Получение всех событий")]
        public async Task GetEventsTest()
        {
            var expected = Enumerable.Range(1, 3).Select(a => CreateEvent()).ToArray();
            var service = GetInitializedService<IEventService, Event>(expected);
            
            var actual = service.GetEvents().ToArray();

            Assert.Equal(expected, actual);
        }

        [Fact(DisplayName = "03. Получение события по ID")]
        public async Task GetEventTest()
        {
            var expected = new Event
            {
                Id = Guid.NewGuid(),
                Title = "GetEventTitle",
                StartAt = DateTime.Now,
                EndAt = DateTime.Now,
                Description = "GetEventDescription",
            };
            var service = GetInitializedService<IEventService, Event>(expected);

            var actual = await service.GetEventAsync(expected.Id);

            Assert.Equal(expected, actual);
        }

        [Theory(DisplayName = "04. Попытка получить событие с несуществующим ID")]
        [InlineData("FEE94FA8-F78B-490B-84F5-80CD75B5A841")]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public async Task GetEventNotExistIdFailTest(Guid id)
        {
            var service = GetService<IEventService>();
            var item = await service.GetEventAsync(id);
            Assert.Null(item);
        }

        [Theory(DisplayName = "05. Попытка обновить событие с несуществующим ID")]
        [InlineData("FEE94FA8-F78B-490B-84F5-80CD75B5A841")]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public async Task TryUpdateEventNotExistIdFailTest(Guid id)
        {
            var service = GetService<IEventService>();
            Assert.False(await service.TryUpdateEventAsync(CreateEvent(e => e.Id = id)));
        }

        [Fact(DisplayName = "06. Обновление существующего события")]
        public async Task TryUpdateEventTest()
        {
            var toUpdate = new Event
            {
                Id = Guid.NewGuid(),
                Title = "BeforeUpdateTitle",
                StartAt = DateTime.Now,
                EndAt = DateTime.Now,
                Description = "BeforeUpdateDescription",
                TotalSeats = 10,
                AvailableSeats = 5,
            };
            var expected = new Event
            {
                Id = toUpdate.Id,
                Title = "AfterUpdatedTitle",
                StartAt = DateTime.Now,
                EndAt = DateTime.Now,
                Description = "AfterUpdateDescription",
                TotalSeats = 11,
                AvailableSeats = 6,
            };
            var service = GetInitializedService<IEventService, Event>(toUpdate);

            Assert.True(await service.TryUpdateEventAsync(expected));
            var actual = await service.GetEventAsync(expected.Id);
            Assert.NotNull(actual);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.StartAt, actual.StartAt);
            Assert.Equal(expected.EndAt, actual.EndAt);
            Assert.Equal(expected.Description, actual.Description);
            Assert.Equal(expected.TotalSeats, actual.TotalSeats);
            Assert.Equal(expected.AvailableSeats, actual.AvailableSeats);
        }

        [Fact(DisplayName = "07. Удаление существующего события")]
        public async Task TryDeleteEventTest()
        {
            var toDelete = CreateEvent();
            var service = GetInitializedService<IEventService, Event>(toDelete);

            Assert.True(await service.TryDeleteEventAsync(toDelete.Id));
            Assert.Null(await service.GetEventAsync(toDelete.Id));
        }
    }
}
