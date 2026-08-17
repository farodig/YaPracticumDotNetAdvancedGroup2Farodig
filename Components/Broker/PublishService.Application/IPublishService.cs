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
        Task PublishAsync<TEvent>(string topic, string key, TEvent message, CancellationToken ct)
            where TEvent : class;
    }
}
