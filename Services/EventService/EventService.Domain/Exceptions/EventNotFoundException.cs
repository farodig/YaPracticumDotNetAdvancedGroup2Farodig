namespace EventService.Domain.Exceptions
{
    /// <summary>
    /// Событие не найдено
    /// </summary>
    public class EventNotFoundException() : Exception("Event not found")
    {
    }
}
