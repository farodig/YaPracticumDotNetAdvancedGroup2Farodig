using LearningWebApi.Entities;
using System.Collections.Concurrent;

namespace LearningWebApi.Repositories
{
    internal class BookingRepository : ConcurrentDictionary<Guid, Booking>, IBookingRepository
    {
        public Booking? Get(Guid id)
        {
            TryGetValue(id, out Booking? item);
            return item;
        }

        public void CreateOrUpdate(Booking item)
        {
            var id = item.Id;
            if (TryGetValue(id, out Booking? oldItem))
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
