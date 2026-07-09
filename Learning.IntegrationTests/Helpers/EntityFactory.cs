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

        public static Booking CreateBooking(Guid? id = null, Guid? eventId = null, BookingStatus? status = null, DateTime? createdAt = null, DateTime? processedAt = null) => new()
        {
            Id = id ?? Guid.NewGuid(),
            EventId = eventId ?? throw new ArgumentNullException(nameof(eventId)),
            Status = status ?? BookingStatus.Pending,
            CreatedAt = createdAt ?? DateTime.Now,
            ProcessedAt = processedAt,
        };

        public static IEventRepository CreateEventRepository(AppDbContext context)
        {
            return new EventRepository(context);
        }

        public static IBookingRepository CreateBookingRepository(AppDbContext context)
        {
            return new BookingRepository(context);
        }
    }
}
