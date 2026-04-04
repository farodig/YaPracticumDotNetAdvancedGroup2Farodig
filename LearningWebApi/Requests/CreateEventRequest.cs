using System.ComponentModel.DataAnnotations;

namespace LearningWebApi.Requests
{
    public class CreateEventRequest
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public DateTime StartAt { get; set; }

        [Required]
        public DateTime EndAt { get; set; }
    }
}
