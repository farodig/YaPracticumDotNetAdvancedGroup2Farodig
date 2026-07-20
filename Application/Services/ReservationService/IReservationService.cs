namespace Application.Services.ReservationService
{
    /// <summary>
    /// Сервис резервирования (или валидации) мест на событии
    /// </summary>
    public interface IReservationService
    {
        /// <summary>
        /// Зарезерировать место на событии
        /// </summary>
        Task ReserveSeatAsync(Guid eventId, Guid personId, CancellationToken cts = default);

        /// <summary>
        /// Освободить место на событии
        /// </summary>
        Task ReleaseSeatAsync(Guid id, CancellationToken cts = default);
    }
}
