using LearningWebApi.Services.BookingService;

namespace LearningWebApi.Models
{
    internal static class BookingFactory
    {
        internal static Booking CreateBooking(Guid eventId) => new()
        {
            Id = Guid.NewGuid(),
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.Now,
            EventId = eventId,
        };
    }
}
