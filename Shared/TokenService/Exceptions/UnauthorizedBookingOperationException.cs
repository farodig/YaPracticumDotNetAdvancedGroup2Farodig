namespace TokenService.Exceptions
{
    /// <summary>
    /// Отсутствие прав на операцию
    /// </summary>
    public class UnauthorizedBookingOperationException() : Exception("Booking operation not permitted")
    {
    }
}
