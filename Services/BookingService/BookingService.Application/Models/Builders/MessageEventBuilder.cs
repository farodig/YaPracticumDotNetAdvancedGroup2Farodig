using BookingService.Domain.Entities;
using BrokerService.Application;
using SharedContracts.Events.BookingEvents;

namespace BookingService.Application.Models.Builders
{
    /// <summary>
    /// Расширение для более удобной сборки и отправки событий сервиса бронирований
    /// </summary>
    internal static class MessageEventBuilder
    {
        /// <summary>
        /// Бронирование создано
        /// </summary>
        internal static async Task PublishBookingCreatedEvent(this IPublishService publisher, Booking data, CancellationToken ct = default)
            => await publisher.PublishAsync(new BookingCreatedEvent()
            {
                Id = data.Id,
                EventId = data.EventId,
            }, ct);

        /// <summary>
        /// Бронирование отменено
        /// </summary>
        internal static async Task PublishBookingCancelEvent(this IPublishService publisher, Booking data, CancelReasonType reason, CancellationToken ct = default)
            => await publisher.PublishAsync(new BookingCancelEvent()
            {
                Id = data.Id,
                EventId = data.EventId,
                ReasonType = reason,
            }, ct);

        /// <summary>
        /// Бронирование подтверждено
        /// </summary>
        internal static async Task PublishBookingConfirmedEvent(this IPublishService publisher, Booking data, CancellationToken ct = default)
            => await publisher.PublishAsync(new BookingConfirmedEvent()
            {
                Id = data.Id,
                EventId = data.EventId,
            }, ct);

        /// <summary>
        /// Бронирование отклонено
        /// </summary>
        internal static async Task PublishBookingRejectedEvent(this IPublishService publisher, Booking data, CancellationToken ct = default)
            => await publisher.PublishAsync(new BookingRejectedEvent()
            {
                Id = data.Id,
                EventId = data.EventId,
            }, ct);
    }
}
