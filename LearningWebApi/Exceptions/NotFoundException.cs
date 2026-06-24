namespace LearningWebApi.Exceptions
{
    /// <summary>
    /// Исключение о неуспешном поиске
    /// </summary>
    public class NotFoundException(string? message) : Exception(message)
    {
    }
}
