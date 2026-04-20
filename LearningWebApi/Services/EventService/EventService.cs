using System.Collections.Concurrent;

namespace LearningWebApi.Services.EventService
{
    internal class EventService : IEventService
    {
        private ConcurrentDictionary<Guid, Event> _events = [];

        public int Count => _events.Count;

        public IEnumerable<Event> GetEvents()
        {
            return _events.Values;
        }

        public Event? GetEvent(Guid id)
        {
            _events.TryGetValue(id, out Event? item);
            return item;
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

        public bool TryUpdateEvent(Event item)
        {
            if (!_events.TryGetValue(item.Id, out Event? oldValue))
            {
                return false;
            }

            _events.TryUpdate(item.Id, item, oldValue);
            return true;
        }

        public bool TryDeleteEvent(Guid id)
        {
            if (!_events.ContainsKey(id))
            {
                return false;
            }

            _events.TryRemove(id, out _);
            return true;
        }
    }
}
