using LearningWebApi.Repositories;

namespace LearningWebApi.Services.EventService
{
    internal class EventService(IEventRepository repository) : IEventService
    {
        private IEventRepository _repository = repository;

        public int Count => _repository.Count;

        public IEnumerable<Event> GetEvents()
        {
            return _repository.Values;
        }

        public Event? GetEvent(Guid id)
        {
            _repository.TryGetValue(id, out Event? item);
            return item;
        }

        public Event CreateEvent(string title, DateTime startAt, DateTime endAt, string? description = null)
        {
            var id = Guid.NewGuid();
            
            return _repository[id] = new Event()
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
            if (!_repository.TryGetValue(item.Id, out Event? oldValue))
            {
                return false;
            }

            _repository.TryUpdate(item.Id, item, oldValue);
            return true;
        }

        public bool TryDeleteEvent(Guid id)
        {
            if (!_repository.ContainsKey(id))
            {
                return false;
            }

            _repository.TryRemove(id, out _);
            return true;
        }
    }
}
