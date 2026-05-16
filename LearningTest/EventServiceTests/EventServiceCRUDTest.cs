using LearningWebApi.Entities;
using static LearningTest.Factories.ServiceFactory;

namespace LearningTest.EventServiceTests
{
    public class EventServiceCRUDTest
    {
        [Fact(DisplayName = "создание события")]
        public void CreateEventTest()
        {
            var expected = new Event
            {
                Title = "ToCreateTitle",
                StartAt = DateTime.Now,
                EndAt = DateTime.Now,
                Description = "ToCreateDescription",
            };
            var actual = CreateEventService()
                .CreateEvent(expected.Title, expected.StartAt, expected.EndAt, expected.Description);

            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.StartAt, actual.StartAt);
            Assert.Equal(expected.EndAt, actual.EndAt);
            Assert.Equal(expected.Description, actual.Description);
        }

        [Fact(DisplayName = "получение всех событий")]
        public void GetEventsTest()
        {
            var expected = new[] {new Event
            {
                Id = Guid.NewGuid(),
                Title = "GetEventsTitle",
                StartAt = DateTime.Now,
                EndAt = DateTime.Now,
                Description = "GetEventsDescription",
            }};
            var actual = CreateEventService(expected)
                .GetEvents();

            Assert.Equal(expected, actual);
        }

        [Fact(DisplayName = "получение события по ID")]
        public void GetEventTest()
        {
            var expected = new Event
            {
                Id = Guid.NewGuid(),
                Title = "GetEventTitle",
                StartAt = DateTime.Now,
                EndAt = DateTime.Now,
                Description = "GetEventDescription",
            };
            var actual = CreateEventService(expected)
                .GetEvent(expected.Id);

            Assert.Equal(expected, actual);
        }

        [Theory(DisplayName = "попытка получить событие с несуществующим ID")]
        [InlineData("FEE94FA8-F78B-490B-84F5-80CD75B5A841")]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public void GetEventNotExistIdFailTest(Guid id)
        {
            var item = CreateEventService()
                .GetEvent(id);
            Assert.Null(item);
        }

        [Theory(DisplayName = "попытка обновить событие с несуществующим ID")]
        [InlineData("FEE94FA8-F78B-490B-84F5-80CD75B5A841")]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public void TryUpdateEventNotExistIdFailTest(Guid id)
        {
            Assert.False(CreateEventService()
                .TryUpdateEvent(new Event
            {
                Id = id,
            }));
        }

        [Fact(DisplayName = "обновление существующего события")]
        public void TryUpdateEventTest()
        {
            var toUpdate = new Event
            {
                Id = Guid.NewGuid(),
                Title = "BeforeUpdateTitle",
                StartAt = DateTime.Now,
                EndAt = DateTime.Now,
                Description = "BeforeUpdateDescription",
            };
            var updated = new Event
            {
                Id = toUpdate.Id,
                Title = "AfterUpdatedTitle",
                StartAt = DateTime.Now,
                EndAt = DateTime.Now,
                Description = "AfterUpdateDescription",
            };
            Assert.True(CreateEventService(toUpdate)
                .TryUpdateEvent(updated));
        }

        [Fact(DisplayName = "удаление существующего события")]
        public void TryDeleteEventTest()
        {
            var toDelete = new Event
            {
                Id = Guid.NewGuid(),
                Title = "ToDeleteTitle",
                StartAt = DateTime.Now,
                EndAt = DateTime.Now,
                Description = "ToDeleteDescription",
            };
            Assert.True(CreateEventService(toDelete)
                .TryDeleteEvent(toDelete.Id));
        }
    }
}
