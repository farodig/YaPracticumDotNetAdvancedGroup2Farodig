namespace Domain.Entities
{
    /// <summary>
    /// Бронирование
    /// </summary>
    public class Booking
    {
        /// <summary>
        /// Уникальный идентификатор брони
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор события, к которому относится бронь
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid PersonId { get; set; }

        /// <summary>
        /// Текущий статус брони
        /// </summary>
        public BookingStatus Status { get; set; }

        /// <summary>
        /// Дата и время создания брони
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Дата и время обработки
        /// </summary>
        public DateTime?  ProcessedAt{ get; set; }

        /// <summary>
        /// Событие к которому относится бронь
        /// </summary>
        public Event Event { get; set; } = null!;

        /// <summary>
        /// Пользователь к которому относится бронь
        /// </summary>
        public Person Person { get; set; } = null!;
    }
}
