using LearningWebApi.Models.Validation;
using System.ComponentModel.DataAnnotations;

namespace LearningWebApi.Models.Requests
{
    /// <summary>
    /// Модель данных изменения события
    /// </summary>
    public class UpdateEventRequest
    {
        // Id требование в route, поэтому отсутствует в модели, т. к. его нет смысла протаскивать повторно,
        // возможно даже есть смысл сделать общую модель для создания и обновления,
        // но бог его знает какие ещё запросы могут быть добавлены

        /// <summary>
        /// Заголовок события
        /// </summary>
        [Required]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Описание события
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Время начала события
        /// </summary>
        [Required]
        [DateLessThanPropertyValidation(nameof(EndAt))]
        [DateGreaterThanNowValidation]
        public DateTime StartAt { get; set; }

        /// <summary>
        /// Время окончания события
        /// </summary>
        [Required]
        [DateGreaterThanPropertyValidation(nameof(StartAt))]
        [DateGreaterThanNowValidation]
        public DateTime EndAt { get; set; }
    }
}
