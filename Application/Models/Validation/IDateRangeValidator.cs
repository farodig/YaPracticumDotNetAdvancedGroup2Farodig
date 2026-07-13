using System.ComponentModel.DataAnnotations;

namespace LearningWebApi.Models.Validation
{
    internal interface IDateRangeValidator
    {
        public DateTime? StartAt { get; set; }

        public DateTime? EndAt { get; set; }

        public IEnumerable<ValidationResult> DateRangeValidate(ValidationContext validationContext)
        {
            if (StartAt >= EndAt)
            {
                yield return new ValidationResult($"Дата начала '{nameof(StartAt)}' должна быть позже даты окончания '{nameof(EndAt)}'", [nameof(StartAt), nameof(EndAt)]);
            }

            if (StartAt <= DateTime.Now)
            {
                yield return new ValidationResult($"Дата начала '{nameof(StartAt)}' должна быть позже текущего времени '{DateTime.Now}'", [nameof(StartAt)]);
            }
        }
    }
}
