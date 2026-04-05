using LearningWebApi.Models.Validation;
using System.ComponentModel.DataAnnotations;

namespace LearningWebApi.Models.Requests
{
    /// <summary>
    /// Модель данных создания события
    /// </summary>
    public class CreateEventRequest
    {
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
