namespace BookingService.Domain.Exceptions
{
    /// <summary>
    /// Бронирование не найдено
    /// </summary>
    public class BookingNotFoundException() : Exception("Booking not found")
    {
    }
}
