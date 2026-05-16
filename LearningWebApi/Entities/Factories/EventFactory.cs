using LearningWebApi.Models.Requests;
using LearningWebApi.Models.Responses;

namespace LearningWebApi.Entities.Factories
{
    /// <summary>
    /// Вспомогательная фабрика для конвертации dto из сервиса в rest и обратно
    /// </summary>
    internal static class EventFactory
    {
        internal static Event CreateEvent(this UpdateEventRequest data, Guid id) => new()
        {
            Id = id,
            Title = data.Title,
            Description = data.Description,
            StartAt = data.StartAt!.Value,
            EndAt = data.EndAt!.Value
        };

        internal static EventResponse ToEventRespose(this Event data) => new()
        {
            Id = data.Id,
            Title = data.Title,
            Description = data.Description,
            StartAt = data.StartAt,
            EndAt = data.EndAt
        };
    }
}
