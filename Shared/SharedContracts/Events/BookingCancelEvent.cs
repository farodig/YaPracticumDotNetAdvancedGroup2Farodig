using SharedContracts.Abstractions;

namespace SharedContracts.Events
{
    /// <summary>
    /// Отмена бронирования
    /// </summary>
    public sealed record BookingCancelEvent : IEvent
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
