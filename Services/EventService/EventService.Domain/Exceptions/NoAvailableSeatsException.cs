namespace EventService.Domain.Exceptions
{
    /// <summary>
    /// Недостаточное количество мест
    /// </summary>
    public class NoAvailableSeatsException() : Exception("No available seats for this event")
    {
    }
}
