using System.ComponentModel.DataAnnotations;

namespace LearningWebApi.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DateGreaterThanNowValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var currentValue = (DateTime?)value
                ?? throw new ArgumentNullException(nameof(value), $"Значение переданное в переменную не может быть приведено к {nameof(DateTime)}");

            if (currentValue <= DateTime.Now)
            {
                return new ValidationResult(ErrorMessage
                    ?? $"{validationContext?.DisplayName ?? "unknown"} должна быть позже текущей даты и времени");
            }

            return ValidationResult.Success;
        }
    }
}
