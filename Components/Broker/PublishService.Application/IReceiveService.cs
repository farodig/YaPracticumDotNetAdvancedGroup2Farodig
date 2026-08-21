using SharedContracts.Abstractions;

namespace PublishService.Application
{
    /// <summary>
    /// Сервис получения событий
    /// </summary>
    public interface IReceiveService
    {
        /// <summary>
        /// Подписываемся на событие
        /// </summary>
        /// <typeparam name="TEvent">Фильтр на событие</typeparam>
        Task StartAsync<TEvent>(Func<TEvent, Task> handler, CancellationToken cts = default)
            where TEvent : class, IEvent;
    }
}
