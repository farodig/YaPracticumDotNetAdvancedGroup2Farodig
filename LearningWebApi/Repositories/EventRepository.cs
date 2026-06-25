using LearningWebApi.Entities;
using System.Collections.Concurrent;

namespace LearningWebApi.Repositories
{
    internal class EventRepository : ConcurrentDictionary<Guid, Event>, IEventRepository
    {
        public void SaveData()
        {
            // заглушка для БД всё в памяти, сохранять не нужно!
        }
    }
}
