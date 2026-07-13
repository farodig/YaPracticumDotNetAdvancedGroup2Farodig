namespace Domain.Exceptions
{
    /// <summary>
    /// Событие не найдено
    /// </summary>
    public class EventNotFoundException() : NotFoundException("Event not found")
    {
    }
}
