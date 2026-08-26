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
        /// Проверка что был вызван метод StringSetAsync, т. е. что то было записано 1 раз (не важно какие данные были переданы, проверка данные не различает, работает только факт)
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
