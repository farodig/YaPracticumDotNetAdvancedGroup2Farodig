using Domain.Entities;
using Application.Models.Responses;

namespace Application.Models.Builders
{
    internal static class BookingBuilder
    {
        internal static Booking CreateBooking(this Guid eventId, Guid personId) => new()
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            PersonId = personId,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.Now,
        };

        internal static BookingResponse BuildBookingResponse(this Booking data) => new()
        {
            Id = data.Id,
            EventId = data.EventId,
            PersonId = data.PersonId,
            Status = data.Status,
            CreatedAt = data.CreatedAt,
            ProcessedAt = data.ProcessedAt,
        };
    }
}
