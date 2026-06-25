using LearningWebApi.Entities;
using LearningWebApi.Repositories;

namespace LearningTest.Factories
{
    internal static class EntityFactory
    {
        public static Event CreateEventTitle(string title) => new()
        {
            Id = Guid.NewGuid(),
            Title = title,
        };

        public static Event CreateEventAvailableSeats(int AvailableSeats = 1) => new()
        {
            Id = Guid.NewGuid(),
            AvailableSeats = AvailableSeats,
        };

        public static Event CreateEventAvailableSeats(this IEventRepository repository, int AvailableSeats = 1)
        {
            var @event = CreateEventAvailableSeats(AvailableSeats);
            repository[@event.Id] = @event;
            return @event;
        }

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
    }
}
