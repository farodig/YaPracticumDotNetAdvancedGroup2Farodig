using LearningWebApi.Validation;
using System.ComponentModel.DataAnnotations;

namespace LearningWebApi.Requests
{
    public class UpdateEventRequest
    {
        // Id требование в route, поэтому отсутствует в модели, т. к. его нет смысла протаскивать повторно,
        // возможно даже есть смысл сделать общую модель для создания и обновления,
        // но бог его знает какие ещё запросы могут быть добавлены

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
