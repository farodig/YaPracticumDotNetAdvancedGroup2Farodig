using Application.Models.Requests;
using Application.Models.Responses;
using Domain.Entities;

namespace UnitTests.Helpers
{
    internal static class EntityFactory
    {
        public static Event CreateEvent(Guid? eventId = null, string? title = null, string? description = null, DateTime? startAt = null, DateTime? endAt = null,
            int? totalSeats = null, int? availableSeats = null) => new()
            {
                Id = eventId ?? Guid.NewGuid(),
                Title = title ?? Guid.NewGuid().ToString(),
                Description = description,
                StartAt = startAt ?? DateTime.Now.AddHours(1),
                EndAt = endAt ?? DateTime.Now.AddHours(2),
                TotalSeats = totalSeats ?? 3,
                AvailableSeats = availableSeats ?? totalSeats ?? 3,
            };

        public static Booking CreateBooking(Guid? bookingId = null, Guid? eventId = null, Guid? personId = null, BookingStatus? status = null, DateTime? createdAt = null, DateTime? endAt = null) => new()
        {
            Id = bookingId ?? Guid.NewGuid(),
            EventId = eventId ?? Guid.NewGuid(),
            PersonId = personId ?? Guid.NewGuid(),
            Status = status ?? BookingStatus.Pending,
            CreatedAt = createdAt ?? DateTime.Now,
            ProcessedAt = endAt,
            
        };

        public static Person CreatePerson(Guid? personId = null) => new()
        {
            Id = personId ?? Guid.NewGuid(),
        };

        public static UpdateEventRequest BuildUpdateEventRequest(this Event item) => new()
        {
            Title = item.Title,
            Description = item.Description,
            StartAt = item.StartAt,
            EndAt = item.EndAt,
            TotalSeats = item.TotalSeats,
            AvailableSeats = item.AvailableSeats,
        };

        public static Event BuildEvent(this EventResponse data) => new()
        {
            Id = data.Id,
            Title = data.Title,
            Description = data.Description,
            StartAt = data.StartAt,
            EndAt = data.EndAt,
            TotalSeats = data.TotalSeats,
            AvailableSeats = data.AvailableSeats,
        };

        public static Booking BuildBooking(this BookingResponse data) => new()
        {
            Id = data.Id,
            EventId = data.EventId,
            Status = data.Status,
            CreatedAt = data.CreatedAt,
            ProcessedAt = data.ProcessedAt,
        };
    }
}
