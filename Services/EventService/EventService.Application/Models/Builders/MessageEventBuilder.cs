using BrokerService.Application;
using SharedContracts.Events.BookingEvents;
using SharedContracts.Events.EventEvents;

namespace EventService.Application.Models.Builders
{
    /// <summary>
    /// Расширение для более удобной сборки и отправки событий сервиса событий
    /// </summary>
    internal static class MessageEventBuilder
    {
        /// <summary>
        /// Места зарезервированы
        /// </summary>
        internal static async Task PublishReserveSeatsEvent(this IPublishService publisher, BookingCreatedEvent data, CancellationToken ct = default)
            => await publisher.PublishAsync(new ReserveSeatsEvent()
            {
                Id = Guid.NewGuid(),
                BookingId = data.BookingId,
                EventId = data.EventId,
            }, ct);

        /// <summary>
        /// Места освобождены
        /// </summary>
        internal static async Task PublishReleaseSeatsEvent(this IPublishService publisher, BookingCancelEvent data, CancellationToken ct = default)
            => await publisher.PublishAsync(new ReleaseSeatsEvent()
            {
                Id = Guid.NewGuid(),
                BookingId = data.BookingId,
                EventId = data.EventId,
            }, ct);

        /// <summary>
        /// Нельзя зарезервировать места
        /// </summary>
        internal static async Task PublishUnableToReserveSeatsEvent(this IPublishService publisher, BookingCreatedEvent data, string details, CancellationToken ct = default)
            => await publisher.PublishAsync(new UnableToReserveSeatsEvent()
            {
                Id = Guid.NewGuid(),
                BookingId = data.BookingId,
                EventId = data.EventId,
                Details = details,
            }, ct);

        /// <summary>
        /// Нельзя освободить места
        /// </summary>
        internal static async Task PublishUnableToReleaseSeatsEvent(this IPublishService publisher, BookingCancelEvent data, string details, CancellationToken ct = default)
            => await publisher.PublishAsync(new UnableToReleaseSeatsEvent()
            {
                Id = Guid.NewGuid(),
                BookingId = data.BookingId,
                EventId = data.EventId,
                Details = details,
            }, ct);
    }
}
