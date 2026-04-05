using System.ComponentModel.DataAnnotations;

namespace LearningWebApi.Models.Validation
{
    /// <summary>
    /// Валидатор поля даты - дата и время должна быть больше текущей даты и времени
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DateGreaterThanNowValidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// Валидация параметра
        /// </summary>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var currentValue = (DateTime?)value;

            if (currentValue <= DateTime.Now)
            {
                return new ValidationResult(ErrorMessage
                    ?? $"{validationContext?.DisplayName ?? "unknown"} должна быть позже текущей даты и времени");
            }

            return ValidationResult.Success;
        }
    }
}
