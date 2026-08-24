using BookingService.Domain.Entities;

namespace BookingService.UnitTest.Helpers
{
    internal static class EntityFactory
    {
        public static Booking CreateBooking(Guid? bookingId = null, Guid? eventId = null, Guid? personId = null, BookingStatus? status = null, DateTime? createdAt = null, DateTime? endAt = null) => new()
        {
            Id = bookingId ?? Guid.NewGuid(),
            EventId = eventId ?? Guid.NewGuid(),
            PersonId = personId ?? Guid.NewGuid(),
            Status = status ?? BookingStatus.Pending,
            CreatedAt = createdAt ?? DateTime.Now,
            ProcessedAt = endAt,
        };
    }
}
