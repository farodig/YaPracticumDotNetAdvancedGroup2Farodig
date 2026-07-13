using Domain.Entities;
using Application.Models.Responses;

namespace Application.Models.Factories
{
    public static class BookingFactory
    {
        public static BookingResponse ToBookingResponse(this Booking data) => new()
        {
            Id = data.Id,
            EventId = data.EventId,
            Status = (Responses.BookingStatus)data.Status,
            CreatedAt = data.CreatedAt,
            ProcessedAt = data.ProcessedAt,
        };
    }
}
