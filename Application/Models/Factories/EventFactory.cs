using Domain.Entities;
using Application.Models.Requests;
using Application.Models.Responses;

namespace Application.Models.Factories
{
    /// <summary>
    /// Вспомогательная фабрика для конвертации dto из сервиса в rest и обратно
    /// </summary>
    public static class EventFactory
    {
        public static Event CreateEvent(this UpdateEventRequest data, Guid id) => new()
        {
            Id = id,
            Title = data.Title,
            Description = data.Description,
            StartAt = data.StartAt!.Value,
            EndAt = data.EndAt!.Value,
            TotalSeats = data.TotalSeats,
            AvailableSeats = data.AvailableSeats,
        };

        public static EventResponse ToEventRespose(this Event data) => new()
        {
            Id = data.Id,
            Title = data.Title,
            Description = data.Description,
            StartAt = data.StartAt,
            EndAt = data.EndAt,
            TotalSeats = data.TotalSeats,
            AvailableSeats = data.AvailableSeats ?? data.TotalSeats, // TODO костыль обновления и слабо описанного тз
        };
    }
}
