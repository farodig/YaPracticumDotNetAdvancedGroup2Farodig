namespace Domain.Exceptions
{
    /// <summary>
    /// Недостаточное количество мест
    /// </summary>
    public class NoAvailableSeatsException() : ABookingException("No available seats for this event")
    {
    }
}
