namespace LearningWebApi.Models.Responses
{
    /// <summary>
    /// Событие
    /// </summary>
    public class EventResponse
    {
        /// <summary>
        /// Идентификатор события
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Заголовок события
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Описание события
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Время начала события
        /// </summary>
        public DateTime StartAt { get; set; }

        /// <summary>
        /// Время окончания события
        /// </summary>
        public DateTime EndAt { get; set; }
    }
}
