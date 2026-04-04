using LearningWebApi.Validation;
using System.ComponentModel.DataAnnotations;

namespace LearningWebApi.Requests
{
    public class CreateEventRequest
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [DateLessThanPropertyValidation(nameof(EndAt))]
        [DateGreaterThanNowValidation]
        public DateTime StartAt { get; set; }

        [Required]
        [DateGreaterThanPropertyValidation(nameof(StartAt))]
        [DateGreaterThanNowValidation]
        public DateTime EndAt { get; set; }
    }
}
