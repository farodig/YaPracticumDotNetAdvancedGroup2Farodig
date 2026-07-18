namespace Domain.Exceptions
{
    /// <summary>
    /// Превышение лимита активных броней
    /// </summary>
    public class ActiveBookingLimitException() : ABookingException("Too many active bookings")
    {
    }
}
