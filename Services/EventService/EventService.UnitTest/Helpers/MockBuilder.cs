using EventService.Application.Abstractions;
using EventService.Domain.Entities;
using EventService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Moq;
using StackExchange.Redis;
using System.Text.Json;

namespace EventService.UnitTest.Helpers
{
    internal static class MockBuilder
    {
        public static IConnectionMultiplexer GetConnectionMultiplexerMock(this Mock<IDatabase> database)
        {
            var moq = new Mock<IConnectionMultiplexer>();

            // Настройка GetDatabase
            moq
                .Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(database.Object);

            return moq.Object;
        }

        public static void InitEventRepositoryData(this Mock<IEventRepository> repository, Guid id, Event item)
        {
            repository
                .Setup(x => x.GetAsync(id))
                .ReturnsAsync(item);
        }

        public static void InitTryUpdateAsync(this Mock<IEventRepository> repository, int count = 1)
        {
            repository
                .Setup(x => x.TryUpdateAsync(It.IsAny<Event>()))
                .ReturnsAsync(count);
        }

        public static void InitTryRemoveAsync(this Mock<IEventRepository> repository, int count = 1)
        {
            repository
                .Setup(x => x.TryRemoveAsync(It.IsAny<Guid>()))
                .ReturnsAsync(count);
        }

        /// <summary>
        /// Инициируем данные в кеше
        /// </summary>
        public static void InitCacheData<TItem>(this Mock<IDatabase> database, string key, TItem item)
        {
            var realkey = $"{typeof(TItem).Name.ToLowerInvariant()}:{key}";
            var value = JsonSerializer.Serialize(item);
            database
                .Setup(x => x.StringGetAsync(realkey))
                .ReturnsAsync(new RedisValue(value));
        }

        /// <summary>
        /// Был вызван метод получения данных из репозитория
        /// </summary>
        public static void AssertRepositoryGetAsyncCallAtOnce(this Mock<IEventRepository> repository)
        {
            repository
                .Verify(x => x.GetAsync(It.IsAny<Guid>())
                , Times.Once);
        }

        /// <summary>
        /// Репозиторий не был задействован
        /// </summary>
        public static void AssertRepositoryGetAsyncCallAtNever(this Mock<IEventRepository> repository)
        {
            repository
                .Verify(x => x.GetAsync(It.IsAny<Guid>())
                , Times.Never);
        }

        /// <summary>
        /// Проверка что был вызван метод записи в кеш
        /// </summary>
        public static void AssertCacheSetAnyDataAtOnce<TItem>(this Mock<IDatabase> database, string key, TItem item, TimeSpan timeToLive)
        {
            var realkey = $"{typeof(TItem).Name.ToLowerInvariant()}:{key}";
            var value = JsonSerializer.Serialize(item);
            database
                .Verify(x => x.StringSetAsync(realkey, value, timeToLive)
                , Times.Once);
        }

        /// <summary>
        /// Проверка того что был вызван метод удаления из кеша только один раз
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="database"></param>
        /// <param name="key"></param>
        public static void AssertCacheDeleteAtOnce<TItem>(this Mock<IDatabase> database, string key)
        {
            var realkey = $"{typeof(TItem).Name.ToLowerInvariant()}:{key}";
            database
                .Verify(x => x.KeyDeleteAsync(realkey)
                , Times.Once);
        }

    }
}
