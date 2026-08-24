using SharedContracts.Abstractions;

namespace SharedContracts.Events.BookingEvents
{
    /// <summary>
    /// Подтверждение бронирования
    /// </summary>
    public sealed record BookingConfirmedEvent : IEvent
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
