using SharedContracts.Abstractions;

namespace SharedContracts.Events
{
    /// <summary>
    /// Бронирование успешно
    /// </summary>
    public sealed record BookingSuccessEvent : IEvent
    {
        /// <summary>
        /// Идентификатор бронирования
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор события
        /// </summary>
        public Guid EventId { get; set; }
        
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid PersonId { get; set; }
        
        /// <summary>
        /// Время подтверждения брони
        /// </summary>
        public DateTime ProcessedAt { get; set; }
    }
}