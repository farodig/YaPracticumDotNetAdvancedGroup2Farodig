namespace SharedContracts.Events
{
    /// <summary>
    /// Событие бронирование подтверждено
    /// </summary>
    public sealed record BookingConfirmedEvent
    {
        /// <summary>
        /// Идентификатор бронирования
        /// </summary>
        public Guid BookingId { get; set; }

        /// <summary>
        /// Идентификатор события
        /// </summary>
        public Guid EventId { get; set; }
        
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid PersonId { get; set; }
        
        /// <summary>
        /// Количество забронированных мест
        /// </summary>
        public int SeatsCount { get; set; }
        
        /// <summary>
        /// Время подтверждения брони
        /// </summary>
        public DateTime ConfirmedAt { get; set; }
    }
}