using CacheService.Infrastructure;
using EventService.Application.Abstractions;
using EventService.Domain.Entities;
using EventService.UnitTest.Helpers;
using static EventService.UnitTest.Helpers.EntityFactory;

namespace EventService.UnitTest
{
    [Trait("Category", "Unit")]
    public class EventSericeCacheTest() : AServiceCollection(isMockRepository: true)
    {
        [Fact(DisplayName = "01. При попадании в кеш репозиторий не вызывается")]
        public async Task GetHitCacheRepoNotCallTest()
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
            // Репозиторий не был задействован
            _mockRepository.AssertRepositoryGetAsyncCallAtNever();
        }

        [Fact(DisplayName = "02. При промахе данные берутся из репозитория и сохраняются в кеш")]
        public async Task CacheChangedOnEventGetTest()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var @event = CreateEvent(eventId: eventId);
            _mockRepository.InitEventRepositoryData(eventId, @event);
            var service = GetService<IEventService>();

            // Act
            await service.GetEventAsync(eventId);

            // Assert
            // Был вызван метод получения данных из репозитория
            _mockRepository.AssertRepositoryGetAsyncCallAtOnce();

            // проверка того что был вызван метод сохранения в кеш только один раз (не важно какие данные)
            _cacheDb.AssertCacheSetAnyDataAtOnce(eventId.ToString(), @event, TimeSpan.FromSeconds(new RedisCacheSettings().GeneralTtlSec));
        }

        [Fact(DisplayName = "03. При мутирующих операциях кеш обновляется")]
        public async Task CacheChangedOnEventUpdateTest()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            _mockRepository.InitEventRepositoryData(eventId, CreateEvent(eventId: eventId));
            _mockRepository.InitTryUpdateAsync();
            var service = GetService<IEventService>();
            var modify = CreateEvent(eventId: eventId);

            // Act
            await service.TryUpdateEventAsync(eventId, modify.BuildUpdateEventRequest());

            // Assert Проверка что был вызван метод записи в кеш
            _cacheDb.AssertCacheSetAnyDataAtOnce(eventId.ToString(), modify, TimeSpan.FromSeconds(new RedisCacheSettings().GeneralTtlSec));
        }

        [Fact(DisplayName = "04. При мутирующих операциях кеш удаляется")]
        public async Task CacheDeleteOnEventDeleteTest()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            _mockRepository.InitEventRepositoryData(eventId, CreateEvent(eventId: eventId));
            _mockRepository.InitTryRemoveAsync();
            var service = GetService<IEventService>();

            // Act
            await service.TryDeleteEventAsync(eventId);

            // Assert - проверка того что был вызван метод удаления из кеша только один раз
            _cacheDb.AssertCacheDeleteAtOnce<Event>(eventId.ToString());
        }
    }
}
