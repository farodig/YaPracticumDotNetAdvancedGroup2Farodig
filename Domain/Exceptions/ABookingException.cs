namespace Domain.Exceptions
{
    /// <summary>
    /// Ошибка бронирования
    /// </summary>
    public abstract class ABookingException(string? message) : Exception(message)
    {
    }
}
