using SharedContracts.Abstractions;

namespace SharedContracts.Events
{
    /// <summary>
    /// Бронирование успешно
    /// </summary>
    public sealed record BookingConfirmEvent : IEvent
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