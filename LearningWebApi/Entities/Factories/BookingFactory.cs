namespace LearningWebApi.Entities.Factories
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
