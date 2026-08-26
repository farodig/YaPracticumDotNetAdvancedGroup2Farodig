using CacheService.Infrastructure;
using EventService.Application.Abstractions;
using EventService.Domain.Entities;
using EventService.UnitTest.Helpers;
using static EventService.UnitTest.Helpers.EntityFactory;

namespace EventService.UnitTest
{
    //Напишите unit-тесты для сервиса, подменяя кеш и репозиторий заглушками.Проверьте следующие сценарии:

    [Trait("Category", "Unit")]
    public class EventSericeCacheTest : AServiceCollection
    {
        [Fact(DisplayName = "01. При попадании в кеш репозиторий не вызывается")]
        public async Task GetFromCacheIgnoreDbTest()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            _cacheDb.InitCacheData(eventId.ToString(), CreateEvent(eventId: eventId));
            var service = GetService<IEventService>();

            // Act
            var eventResponse = await service.GetEventAsync(eventId);

            // Assert
            Assert.NotNull(eventResponse);
            Assert.Equal(eventId, eventResponse.Id);
        }

        [Fact(DisplayName = "02. При промахе данные берутся из репозитория и сохраняются в кеш")]
        public async Task CacheChangedOnEventGetTest()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var @event = CreateEvent(eventId: eventId);
            var service = GetInitializedService<IEventService, Event>(@event);

            // Act
            await service.GetEventAsync(eventId);

            // Assert - проверка того что был вызван метод сохранения в кеш только один раз (не важно какие данные)
            _cacheDb.AssertCacheSetAnyDataAtOnce(eventId.ToString(), @event, TimeSpan.FromSeconds(new RedisCacheSettings().GeneralTtlSec));
        }

        [Fact(DisplayName = "03. При мутирующих операциях кеш обновляется")]
        public async Task CacheChangedOnEventUpdateTest()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var service = GetInitializedService<IEventService, Event>(CreateEvent(eventId: eventId));
            var modify = CreateEvent(eventId: eventId);

            // Act
            await service.TryUpdateEventAsync(eventId, modify.BuildUpdateEventRequest());

            // Assert - проверка того что был вызван метод сохранения в кеш только один раз (не важно какие данные)
            _cacheDb.AssertCacheSetAnyDataAtOnce(eventId.ToString(), modify, TimeSpan.FromSeconds(new RedisCacheSettings().GeneralTtlSec));
        }

        [Fact(DisplayName = "04. При мутирующих операциях кеш удаляется")]
        public async Task CacheDeleteOnEventDeleteTest()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var service = GetInitializedService<IEventService, Event>(CreateEvent(eventId: eventId));

            // Act
            await service.TryDeleteEventAsync(eventId);

            // Assert - проверка того что был вызван метод удаления из кеша только один раз
            _cacheDb.AssertCacheDeleteAtOnce<Event>(eventId.ToString());
        }
    }
}
