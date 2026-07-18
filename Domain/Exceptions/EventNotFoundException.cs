namespace Domain.Exceptions
{
    /// <summary>
    /// Событие не найдено
    /// </summary>
    public class EventNotFoundException() : ANotFoundException("Event not found")
    {
    }
}
