using SharedContracts.Abstractions;

namespace SharedContracts.Events.EventEvents
{
    /// <summary>
    /// Не удалось зарезервировать место
    /// </summary>
    public sealed record UnableToReserveSeatsEvent : IEvent
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

        /// <summary>
        /// Детали проблемы
        /// </summary>
        public string Details { get; set; } = null!;
    }
}
