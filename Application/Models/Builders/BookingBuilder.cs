using Domain.Entities;
using Application.Models.Responses;

namespace Application.Models.Builders
{
    internal static class BookingBuilder
    {
        internal static Booking BuildBooking(this Guid eventId) => new()
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.Now,
        };

        internal static BookingResponse BuildBookingResponse(this Booking data) => new()
        {
            Id = data.Id,
            EventId = data.EventId,
            Status = data.Status,
            CreatedAt = data.CreatedAt,
            ProcessedAt = data.ProcessedAt,
        };
    }
}
