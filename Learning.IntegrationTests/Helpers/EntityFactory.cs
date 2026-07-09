using LearningWebApi.DataAccess;
using LearningWebApi.Entities;
using LearningWebApi.Repositories;

namespace Learning.IntegrationTests.Helpers
{
    internal static class EntityFactory
    {
        public static Event CreateEvent(Guid? eventId = null, string? title = null, DateTime? startAt = null, DateTime? endAt = null,
            int? totalSeats = null, int? availableSeats = null, string? description = null) => new()
        {
            Id = eventId ?? Guid.NewGuid(),
            Title = title ??  Guid.NewGuid().ToString(),
            StartAt = startAt ?? DateTime.Now.AddHours(1),
            EndAt = endAt ?? DateTime.Now.AddHours(2),
            TotalSeats = totalSeats ?? 3,
            AvailableSeats = availableSeats ?? totalSeats ?? 3,
            Description = description,
        };

        public static Booking CreateBooking() => new()
        {
            Id = Guid.NewGuid(),
        };

        public static IEventRepository CreateEventRepository(AppDbContext context)
        {
            return new EventRepository(context);
        }
    }
}
