using LearningWebApi.Entities;
using static LearningTest.Factories.ServiceFactory;

namespace LearningTest.EventServiceTests
{
    public class EventServiceCRUDTest
    {
        [Fact(DisplayName = "создание события")]
        public async Task CreateEventTest()
        {
            var expected = new Event
            {
                Title = "ToCreateTitle",
                StartAt = DateTime.Now,
                EndAt = DateTime.Now,
                Description = "ToCreateDescription",
                TotalSeats = 10,
                AvailableSeats = 10,
            };
            var service = await CreateEventService();
            var actual = await service
                .CreateEventAsync(expected.Title,
                expected.StartAt,
                expected.EndAt,
                expected.TotalSeats,
                expected.Description);

            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.StartAt, actual.StartAt);
            Assert.Equal(expected.EndAt, actual.EndAt);
            Assert.Equal(expected.Description, actual.Description);
            Assert.Equal(expected.TotalSeats, actual.TotalSeats);
            Assert.Equal(expected.AvailableSeats, actual.AvailableSeats);
        }

        [Fact(DisplayName = "получение всех событий")]
        public async Task GetEventsTest()
        {
            var expected = new[] {new Event
            {
                Id = Guid.NewGuid(),
                Title = "GetEventsTitle",
                StartAt = DateTime.Now,
                EndAt = DateTime.Now,
                Description = "GetEventsDescription",
            }};
            var service = await CreateEventService(expected);
            var actual = service.GetEvents();

            Assert.Equal(expected, actual);
        }

        [Fact(DisplayName = "получение события по ID")]
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

            var service = await CreateEventService(expected);
            var actual = await service.GetEventAsync(expected.Id);

            Assert.Equal(expected, actual);
        }

        [Theory(DisplayName = "попытка получить событие с несуществующим ID")]
        [InlineData("FEE94FA8-F78B-490B-84F5-80CD75B5A841")]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public async Task GetEventNotExistIdFailTest(Guid id)
        {
            var service = await CreateEventService();
            var item = await service.GetEventAsync(id);
            Assert.Null(item);
        }

        [Theory(DisplayName = "попытка обновить событие с несуществующим ID")]
        [InlineData("FEE94FA8-F78B-490B-84F5-80CD75B5A841")]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public async Task TryUpdateEventNotExistIdFailTest(Guid id)
        {
            var service = await CreateEventService();
            Assert.False(await service.TryUpdateEventAsync(new Event
            {
                Id = id,
            }));
        }

        [Fact(DisplayName = "обновление существующего события")]
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
            var updated = new Event
            {
                Id = toUpdate.Id,
                Title = "AfterUpdatedTitle",
                StartAt = DateTime.Now,
                EndAt = DateTime.Now,
                Description = "AfterUpdateDescription",
                TotalSeats = 11,
                AvailableSeats = 6,
            };
            var service = await CreateEventService(toUpdate);
            Assert.True(await service.TryUpdateEventAsync(updated));
        }

        [Fact(DisplayName = "удаление существующего события")]
        public async Task TryDeleteEventTest()
        {
            var toDelete = new Event
            {
                Id = Guid.NewGuid(),
                Title = "ToDeleteTitle",
                StartAt = DateTime.Now,
                EndAt = DateTime.Now,
                Description = "ToDeleteDescription",
            };
            var service = await CreateEventService(toDelete);
            await service.TryDeleteEventAsync(toDelete.Id);
            Assert.Null(await service.GetEventAsync(toDelete.Id));
        }
    }
}
