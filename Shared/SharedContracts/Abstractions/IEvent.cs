namespace SharedContracts.Abstractions
{
    public interface IEvent
    {
        /// <summary>
        /// Идентификатор сообщения
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Время обработки
        /// </summary>
        public DateTime ProcessedAt { get; set; }
    }
}
