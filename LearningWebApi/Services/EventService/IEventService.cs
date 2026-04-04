using LearningWebApi.Models;

namespace LearningWebApi.Services.EventService
{
    public interface IEventService
    {
        IEnumerable<Event> GetEvents();
        Event? GetEvent(Guid id);
        Event CreateEvent(string title, DateTime startAt, DateTime endAt, string? description = null);
        bool TryUpdateEvent(Event item);
        bool TryDeleteEvent(Guid id);
    }
}
