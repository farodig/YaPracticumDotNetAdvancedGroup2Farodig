using LearningWebApi.Entities;

namespace Learning.IntegrationTests.Helpers
{
    internal static class EntityFactory
    {
        public static Event CreateEvent(Guid? eventId = null, string? title = null, DateTime? startAt = null, DateTime? endAt = null,
            int? totalSeats = null, int? availableSeats = null) => new()
        {
            Id = eventId ?? Guid.NewGuid(),
            Title = title ??  Guid.NewGuid().ToString(),
            StartAt = startAt ?? DateTime.Now.AddHours(1),
            EndAt = endAt ?? DateTime.Now.AddHours(2),
            TotalSeats = totalSeats ?? 3,
            AvailableSeats = availableSeats ?? totalSeats ?? 3,
        };

        public static Booking CreateBooking() => new()
        {
            Id = Guid.NewGuid(),
        };
    }
}
