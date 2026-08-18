using SharedContracts.Abstractions;

namespace SharedContracts.Events
{
    /// <summary>
    /// Бронирование окончилось неудачей
    /// </summary>
    public sealed record BookingFailureEvent : IEvent
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
        /// Время отклонения брони
        /// </summary>
        public DateTime ProcessedAt { get; set; }
        
        // TODO: Код ошибки - можно добавить, а зачем?
    }
}
