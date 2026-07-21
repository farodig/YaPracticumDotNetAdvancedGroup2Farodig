namespace Domain.Exceptions
{
    /// <summary>
    /// Бронирование не найдено
    /// </summary>
    public class BookingNotFoundException() : ANotFoundException("Booking not found")
    {
    }
}
