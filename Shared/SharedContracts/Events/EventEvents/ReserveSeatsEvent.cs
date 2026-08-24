using SharedContracts.Abstractions;

namespace SharedContracts.Events.EventEvents
{
    /// <summary>
    /// Место освобождено
    /// </summary>
    public sealed record ReserveSeatsEvent : IEvent
    {
        /// <summary>
        /// Идентификатор сообщения
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор бронирования
        /// </summary>
        public Guid BookingId { get; set; }

        /// <summary>
        /// Идентификатор события
        /// </summary>
        public Guid EventId { get; set; }
    }
}
