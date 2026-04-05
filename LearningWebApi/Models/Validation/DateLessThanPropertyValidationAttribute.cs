using System.ComponentModel.DataAnnotations;

namespace LearningWebApi.Models.Validation
{
    /// <summary>
    /// Валидатор поля даты - дата и время должна быть меньше параметра с которым сравниваем
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DateLessThanPropertyValidationAttribute(string comparisonProperty) : ValidationAttribute
    {
        private readonly string _comparisonProperty = comparisonProperty;

        /// <summary>
        /// Валидация параметра
        /// </summary>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var currentValue = (DateTime?)value;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
            var comparisonValue = (DateTime?)property?.GetValue(validationContext.ObjectInstance);

            if (currentValue >= comparisonValue)
            {
                return new ValidationResult(ErrorMessage ??
                    $"{validationContext?.DisplayName ?? "unknown"} должна быть раньше {_comparisonProperty}");
            }

            return ValidationResult.Success;
        }
    }
}
