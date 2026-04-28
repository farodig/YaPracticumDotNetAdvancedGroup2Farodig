using LearningWebApi.Services.EventService;
using System.Collections.Concurrent;

namespace LearningWebApi.Repositories
{
    internal class EventRepository : ConcurrentDictionary<Guid, Event>, IEventRepository
    {
    }
}
