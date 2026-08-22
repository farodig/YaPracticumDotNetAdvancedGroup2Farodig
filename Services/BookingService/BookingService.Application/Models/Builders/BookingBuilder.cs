using BookingService.Application.Models.Responses;
using BookingService.Domain.Entities;
using SharedContracts.Events;

namespace BookingService.Application.Models.Builders
{
    internal static class BookingBuilder
    {
        internal static Booking CreateBooking(Guid eventId, Guid personId) => new()
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            PersonId = personId,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.Now,
        };

        internal static BookingResponse ToResponse(this Booking data) => new()
        {
            Id = data.Id,
            EventId = data.EventId,
            PersonId = data.PersonId,
            Status = data.Status,
            CreatedAt = data.CreatedAt,
            ProcessedAt = data.ProcessedAt,
        };

        internal static BookingCreatedEvent ToBookingCreatedEvent(this Booking data) => new()
        {
            Id = data.Id,
            EventId = data.EventId,
        };

        internal static BookingCancelEvent ToBookingCancelEvent(this Booking data, CancelReasonType reason) => new()
        {
            Id = data.Id,
            EventId = data.EventId,
            ReasonType = reason,
        };
    }
}
