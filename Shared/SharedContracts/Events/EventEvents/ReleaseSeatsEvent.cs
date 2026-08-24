using SharedContracts.Abstractions;

namespace SharedContracts.Events.EventEvents
{
    /// <summary>
    /// Место занято
    /// </summary>
    public sealed record ReleaseSeatsEvent : IEvent
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
