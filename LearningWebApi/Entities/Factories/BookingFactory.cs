using LearningWebApi.Models.Responses;

namespace LearningWebApi.Entities.Factories
{
    internal static class BookingFactory
    {
        internal static Booking CreateBooking(Guid eventId) => new()
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.Now,
        };

        internal static BookingResponse ToBookingRespose(this Booking data) => new()
        {
            Id = data.Id,
            EventId = data.EventId,
            Status = (Models.Responses.BookingStatus)data.Status,
            CreatedAt = data.CreatedAt,
            ProcessedAt = data.ProcessedAt,
        };
    }
}
