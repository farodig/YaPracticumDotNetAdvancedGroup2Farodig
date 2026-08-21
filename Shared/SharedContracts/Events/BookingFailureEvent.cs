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
        
        // TODO: Код ошибки - можно добавить, а зачем?
    }
}
