using Domain.Entities;

namespace Application.Services.BookingService
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
    }
}
