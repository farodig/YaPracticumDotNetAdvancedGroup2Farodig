
using Domain.Entities;

namespace Application.Models.Responses
{
    /// <summary>
    /// Бронирование
    /// </summary>
    public class BookingResponse
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
        /// Идентификатор пользователя на которого создана бронь
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
        public DateTime? ProcessedAt { get; set; }
    }
}
