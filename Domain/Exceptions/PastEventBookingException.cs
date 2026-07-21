namespace Domain.Exceptions
{
    /// <summary>
    /// Бронирование прошедшего события
    /// </summary>
    public class PastEventBookingException() : ABookingException("Booking a past event")
    {
    }
}
