using LearningWebApi.Models;
using System.Collections.Concurrent;

namespace LearningWebApi.Services.EventService
{
    internal class EventService : IEventService
    {
        private ConcurrentDictionary<Guid, Event> _events = [];

        public IEnumerable<Event> GetEvents()
        {
            return _events.Values;
        }

        public Event GetEvent(Guid id)
        {
            return _events[id];
        }

        public Event CreateEvent(string title, DateTime startAt, DateTime endAt, string? description = null)
        {
            var id = Guid.NewGuid();
            
            return _events[id] = new Event()
            {
                Id = id,
                Title = title,
                Description = description,
                StartAt = startAt,
                EndAt = endAt,
            };
        }

        public void CancelEvent(Guid id)
        {
            _events.TryRemove(id, out _);
        }
    }
}
