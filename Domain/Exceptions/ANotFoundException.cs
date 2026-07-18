namespace Domain.Exceptions
{
    /// <summary>
    /// Исключение о неуспешном поиске
    /// </summary>
    public class ANotFoundException(string? message) : Exception(message)
    {
    }
}
