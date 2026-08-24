using SharedContracts.Abstractions;

namespace SharedContracts.Events.BookingEvents
{
    /// <summary>
    /// Отклонение бронирования
    /// </summary>
    public sealed record class BookingRejectedEvent : IEvent
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
