namespace Domain.Exceptions
{
    /// <summary>
    /// Отсутствие прав на операцию
    /// </summary>
    public class UnauthorizedBookingOperationException() : ABookingException("Booking operation not permitted")
    {
    }
}
