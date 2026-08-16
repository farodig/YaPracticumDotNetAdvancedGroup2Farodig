namespace EventService.Domain.Exceptions
{
    /// <summary>
    /// Бронирование прошедшего события
    /// </summary>
    public class PastEventReserveException() : Exception("Booking a past event")
    {
    }
}
