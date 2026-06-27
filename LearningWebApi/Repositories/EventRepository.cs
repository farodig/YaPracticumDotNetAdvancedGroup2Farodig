using LearningWebApi.Entities;
using System.Collections.Concurrent;

namespace LearningWebApi.Repositories
{
    internal class EventRepository : ConcurrentDictionary<Guid, Event>, IEventRepository
    {
        public Event? Get(Guid id)
        {
            TryGetValue(id, out Event? @event);
            return @event;
        }

        public void CreateOrUpdate(Event item)
        {
            var id = item.Id;
            if (TryGetValue(id, out Event? oldItem))
            {
                TryUpdate(id, item, oldItem);
            }
            else
            {
                TryAdd(id, item);
            }
        }

        public void Remove(Guid id)
        {
            TryRemove(id, out _);
        }
    }
}
