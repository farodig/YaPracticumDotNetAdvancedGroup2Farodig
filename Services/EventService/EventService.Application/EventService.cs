using Application.Models.Requests;
using CacheService.Application;
using EventService.Application.Abstractions;
using EventService.Application.Models.Builders;
using EventService.Application.Models.Responses;
using EventService.Domain.Entities;

namespace EventService.Application
{
    public class EventService(IEventRepository repository, ICacheServiceFactory cacheServiceFactory) : IEventService
    {
        private readonly IEventRepository _repository = repository;
        private readonly ICacheService<Event> _cache = cacheServiceFactory.CreateCacheService<Event>();

        public async Task<PaginatedResult> GetEventsAsync(
            int page,
            int pageSize,
            string? title = null, 
            DateTime? from = null, 
            DateTime? to = null,
            CancellationToken cts = default)
        {
            var collection = await _repository.GetEventsAsync(page, pageSize, title, from, to, cts);

            return new PaginatedResult
            {
                Items = [.. collection.Select(a => a.BuildEventRespose())],
                PageNumber = page,
                TotalCount = collection.Count(),
            };
        }

        public async Task<IEnumerable<EventResponse>> GetTop10EventsAsync(CancellationToken cts = default)
        {
            var collection = await GetTop10EventsInternalAsync(cts);
            return collection.Select(a => a.BuildEventRespose());
        }

        public async Task<EventResponse?> GetEventAsync(Guid id, CancellationToken cts = default)
        {
            var item = await GetEventInternalAsync(id, cts);
            return item?.BuildEventRespose();
        }

        public async Task<EventResponse> CreateEventAsync(string title, DateTime startAt, DateTime endAt, int totalSeats, string? description = null, CancellationToken cts = default)
        {
            var item = new Event()
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = description,
                StartAt = startAt,
                EndAt = endAt,
                TotalSeats = totalSeats,
                AvailableSeats = totalSeats,
            };

            await CreateEventInternalAsync(item, cts);

            return item.BuildEventRespose();
        }

        public async Task<bool> TryUpdateEventAsync(Guid id, UpdateEventRequest item, CancellationToken cts = default)
        {
            var update = item.BuildEvent(id);
            return await TryUpdateEventInternalAsync(update, cts);
        }

        public async Task<bool> TryDeleteEventAsync(Guid id, CancellationToken cts = default)
        {
            if (await _repository.TryRemoveAsync(id, cts) > 0)
            {
                await _cache.DeleteAsync(id);
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task<IEnumerable<Event>> GetTop10EventsInternalAsync(CancellationToken cts = default)
        {
            const int TopCount = 10;
            const int UpdateIntervalHours = 1;

            if (await _cache.GetCollectionAsync(CacheKeys.TopPopularCollection) is not IEnumerable<Event> collection || !collection.Any())
            {
                collection = await _repository.GetTopPopularEventsAsync(TopCount, cts);
                await _cache.SetCollectionAsync(CacheKeys.TopPopularCollection, collection, TimeSpan.FromHours(UpdateIntervalHours));
            }

            return collection;
        }

        private async Task<Event?> GetEventInternalAsync(Guid id, CancellationToken cts = default)
        {
            Event? item = await _cache.GetAsync(id);
            if (item is not null)
            {
                return item;
            }

            item = await _repository.GetAsync(id, cts);
            if (item is not null)
            {
                await _cache.SetAsync(id, item);
            }

            return item;
        }

        private async Task CreateEventInternalAsync(Event created, CancellationToken cts = default)
        {
            await _repository.CreateAsync(created, cts);
            await _cache.SetAsync(created.Id, created);
        }

        private async Task<bool> TryUpdateEventInternalAsync(Event update, CancellationToken cts = default)
        {
            if (await _repository.TryUpdateAsync(update, cts) > 0)
            {
                await _cache.SetAsync(update.Id, update);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
