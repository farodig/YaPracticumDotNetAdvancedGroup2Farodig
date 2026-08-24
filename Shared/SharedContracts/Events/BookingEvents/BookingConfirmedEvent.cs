using SharedContracts.Abstractions;

namespace SharedContracts.Events.BookingEvents
{
    /// <summary>
    /// Подтверждение бронирования
    /// </summary>
    public sealed record BookingConfirmedEvent : IEvent
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
