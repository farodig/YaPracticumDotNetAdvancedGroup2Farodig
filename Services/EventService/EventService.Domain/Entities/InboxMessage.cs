namespace EventService.Domain.Entities
{
    /// <summary>
    /// Входящее сообщение
    /// </summary>
    public class InboxMessage
    {
        /// <summary>
        /// Идентификатор сообщения
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Время получения сообщения
        /// </summary>
        public DateTime ReceivedAt { get; set; }
    }
}
