using LearningWebApi.Entities;

namespace LearningTest.Factories
{
    internal static class EntityFactory
    {
        public static Event CreateEvent() => new()
        {
            Id = Guid.NewGuid(),
        };

        public static Event CreateEventTitle(string title) => new()
        {
            Id = Guid.NewGuid(),
            Title = title,
        };

        public static Event CreateEventAvailableSeats(int AvailableSeats = 1) => new()
        {
            Id = Guid.NewGuid(),
            AvailableSeats = AvailableSeats,
            TotalSeats = AvailableSeats,
        };

        public static Event CreateEventStartAt(DateTime startAt) => new()
        {
            Id = Guid.NewGuid(),
            StartAt = startAt,
        };

        public static Event CreateEventEndAt(DateTime endAt) => new()
        {
            Id = Guid.NewGuid(),
            EndAt = endAt,
        };

        public static Event CreateEvent(string title, DateTime startAt, DateTime endAt, string? description = null) => new()
        {
            Id = Guid.NewGuid(),
            Title = title,
            StartAt = startAt,
            EndAt = endAt,
            Description = description,
        };

        public static Booking CreateBooking() => new()
        {
            Id = Guid.NewGuid(),
        };
    }
}
