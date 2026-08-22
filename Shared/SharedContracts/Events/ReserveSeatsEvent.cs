using SharedContracts.Abstractions;

namespace SharedContracts.Events
{
    /// <summary>
    /// Место освобождено
    /// </summary>
    public sealed record ReserveSeatsEvent : IEvent
    {
        /// <summary>
        /// Идентификатор бронирования
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор события
        /// </summary>
        public Guid EventId { get; set; }
    }
}
