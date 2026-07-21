using Application.Abstractions;
using Application.Components;
using Domain.Entities;
using Infrastructure.DataAccess;
using Infrastructure.Repositories;

namespace IntegrationTests.Helpers
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

        public static Booking CreateBooking(Guid? id = null, Guid? eventId = null, Guid? personId = null, BookingStatus? status = null, DateTime? createdAt = null, DateTime? processedAt = null) => new()
        {
            Id = id ?? Guid.NewGuid(),
            EventId = eventId ?? throw new ArgumentNullException(nameof(eventId)),
            PersonId = personId ?? throw new ArgumentNullException(nameof(personId)),
            Status = status ?? BookingStatus.Pending,
            CreatedAt = createdAt ?? DateTime.Now,
            ProcessedAt = processedAt,
        };

        public static Person CreatePerson(Guid? id = null, string? login = null, string? password = null, PersonRole? role = null) => new()
        {
            Id = id ?? Guid.NewGuid(),
            Login = login ?? "login",
            Role = role ?? PersonRole.Admin,
            PasswordHash = new SHA256PasswordHasher().GenerateHash(password ?? "password"),
        };

        public static IEventRepository CreateEventRepository(AppDbContext context)
        {
            return new EventRepository(context);
        }

        public static IBookingRepository CreateBookingRepository(AppDbContext context)
        {
            return new BookingRepository(context);
        }

        public static IPersonRepository CreatePersonRepository(AppDbContext context)
        {
            return new PersonRepository(context);
        }
    }
}
