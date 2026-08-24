using SharedContracts.Abstractions;

namespace BrokerService.Application
{
    /// <summary>
    /// Сервис получения событий
    /// </summary>
    public interface IReceiveService<TEvent> : IDisposable
        where TEvent : class, IEvent
    {
        /// <summary>
        /// Подписываемся на событие и запускаем прослушку
        /// </summary>
        /// <typeparam name="TEvent">Фильтр на событие</typeparam>
        Task StartAsync(Func<TEvent, CancellationToken, Task> handler, CancellationToken cts = default);

        /// <summary>
        /// Остановить прослушку
        /// </summary>
        Task StopAsync(CancellationToken cts = default);
    }
}
