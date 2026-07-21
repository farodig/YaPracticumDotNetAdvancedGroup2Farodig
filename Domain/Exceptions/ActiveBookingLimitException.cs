namespace Domain.Exceptions
{
    /// <summary>
    /// Превышение лимита активных броней
    /// </summary>
    public class ActiveBookingLimitException(int limit) : ABookingException($"Too many active bookings. No more than {limit} bookings")
    {
    }
}
