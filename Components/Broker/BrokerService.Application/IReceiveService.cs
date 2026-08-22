using SharedContracts.Abstractions;

namespace BrokerService.Application
{
    /// <summary>
    /// Сервис получения событий
    /// </summary>
    public interface IReceiveService : IDisposable
    {
        /// <summary>
        /// Подписываемся на событие и запускаем прослушку
        /// </summary>
        /// <typeparam name="TEvent">Фильтр на событие</typeparam>
        Task StartAsync<TEvent>(Func<TEvent, CancellationToken, Task> handler, CancellationToken cts = default)
            where TEvent : class, IEvent;

        /// <summary>
        /// Остановить прослушку
        /// </summary>
        Task StopAsync(CancellationToken cts = default);
    }
}
