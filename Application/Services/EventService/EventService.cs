using Application.Abstractions;
using Application.Models.Builders;
using Application.Models.Requests;
using Application.Models.Responses;
using Domain.Entities;

namespace Application.Services.EventService
{
    public class EventService(IEventRepository repository) : IEventService
    {
        private readonly IEventRepository _repository = repository;

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

        public async Task<EventResponse?> GetEventAsync(Guid id, CancellationToken cts = default)
        {
            var item = await _repository.GetAsync(id, cts);
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

            await _repository.CreateAsync(item, cts);

            return item.BuildEventRespose();
        }

        public async Task<bool> TryUpdateEventAsync(Guid id, UpdateEventRequest item, CancellationToken cts = default)
        {
            var toUpdate = item.BuildEvent(id);

            return await _repository.TryUpdateAsync(toUpdate, cts) > 0;
        }

        public async Task<bool> TryDeleteEventAsync(Guid id, CancellationToken cts = default)
        {
            return await _repository.TryRemoveAsync(id, cts) > 0;
        }
    }
}
