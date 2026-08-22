using Application.Models.Requests;
using EventService.Application.Models.Responses;
using EventService.Domain.Entities;
using SharedContracts.Events;

namespace EventService.Application.Models.Builders
{
    /// <summary>
    /// Вспомогательная фабрика для конвертации dto из сервиса в rest и обратно
    /// </summary>
    public static class EventBuilder
    {
        public static Event BuildEvent(this UpdateEventRequest data, Guid id) => new()
        {
            Id = id,
            Title = data.Title,
            Description = data.Description,
            StartAt = data.StartAt!.Value,
            EndAt = data.EndAt!.Value,
            TotalSeats = data.TotalSeats,
            AvailableSeats = data.AvailableSeats,
        };

        public static EventResponse BuildEventRespose(this Event data) => new()
        {
            Id = data.Id,
            Title = data.Title,
            Description = data.Description,
            StartAt = data.StartAt,
            EndAt = data.EndAt,
            TotalSeats = data.TotalSeats,
            AvailableSeats = data.AvailableSeats ?? data.TotalSeats,
        };

        public static ReserveSeatsEvent BuildReserveSeatsEvent(this BookingCreatedEvent data) => new()
        {
            Id = data.Id,
            EventId = data.EventId,
        };

        public static ReleaseSeatsEvent BuildReleaseSeatsEvent(this BookingCancelEvent data) => new()
        {
            Id = data.Id,
            EventId = data.EventId,
        };

        public static UnableToChangeSeatsEvent BuildUnableToChangeSeatsEvent(this BookingCreatedEvent data, string details) => new()
        {
            Id = data.Id,
            EventId = data.EventId,
            Details = details,
        };

        public static UnableToChangeSeatsEvent BuildUnableToChangeSeatsEvent(this BookingCancelEvent data, string details) => new()
        {
            Id = data.Id,
            EventId = data.EventId,
            Details = details,
        };
    }
}
