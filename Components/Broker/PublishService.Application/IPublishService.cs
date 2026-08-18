using SharedContracts.Abstractions;

namespace PublishService.Application
{
    /// <summary>
    /// Сервис публикации событий/сообщений
    /// </summary>
    public interface IPublishService
    {
        /// <summary>
        /// Опубликовать событие/сообщение
        /// </summary>
        Task PublishAsync<TEvent>(TEvent message, CancellationToken ct = default)
            where TEvent : class, IEvent;
    }
}
