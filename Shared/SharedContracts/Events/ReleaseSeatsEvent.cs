using SharedContracts.Abstractions;

namespace SharedContracts.Events
{
    /// <summary>
    /// Место занято
    /// </summary>
    public sealed record ReleaseSeatsEvent : IEvent
    {
        /// <summary>
        /// Идентификатор бронирования
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор события
        /// </summary>
        public Guid EventId { get; set; }
    }
}
