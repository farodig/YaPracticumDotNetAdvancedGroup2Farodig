using LearningWebApi.Models.Validation;
using System.ComponentModel.DataAnnotations;

namespace LearningWebApi.Models.Requests
{
    /// <summary>
    /// Модель данных создания события
    /// </summary>
    public class CreateEventRequest : IValidatableObject, IDateRangeValidator
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
        public DateTime? StartAt { get; set; }

        /// <summary>
        /// Время окончания события
        /// </summary>
        [Required]
        public DateTime? EndAt { get; set; }

        /// <summary>
        /// Валидация
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return ((IDateRangeValidator)this).DateRangeValidate(validationContext);
        }
    }
}
