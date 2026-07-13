using Application.Repositories;
using Domain.Entities;
using Domain.Exceptions;
using LearningWebApi.Entities;

namespace LearningWebApi.Services.EventService
{
    internal class EventService(IEventRepository repository) : IEventService
    {
        private readonly IEventRepository _repository = repository;

        public async Task<IEnumerable<Event>> GetEventsAsync(
            int page,
            int pageSize,
            string? title = null, 
            DateTime? from = null, 
            DateTime? to = null,
            CancellationToken cts = default)
        {
            return await _repository.GetEventsAsync(page, pageSize, title, from, to, cts);
        }

        public async Task<Event?> GetEventAsync(Guid id, CancellationToken cts = default)
        {
            return await _repository.GetAsync(id, cts);
        }

        public async Task<Event> CreateEventAsync(string title, DateTime startAt, DateTime endAt, int totalSeats, string? description = null, CancellationToken cts = default)
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

            return item;
        }

        public async Task<bool> TryUpdateEventAsync(Event item, CancellationToken cts = default)
        {
            return await _repository.TryUpdateAsync(item, cts) > 0;
        }

        public async Task<bool> TryDeleteEventAsync(Guid id, CancellationToken cts = default)
        {
            return await _repository.TryRemoveAsync(id, cts) > 0;
        }

        public async Task ReserveSeatAsync(Guid id, CancellationToken cts = default)
        {
            // Получить событие из хранилища
            if (await _repository.GetAsync(id, cts) is not Event @event) throw new EventNotFoundException();

            // Попытка зарезервировать свободное место
            if (!@event.TryReserveSeats()) throw new NoAvailableSeatsException();

            await _repository.TryUpdateContextAsync(@event, cts);
        }

        public async Task ReleaseSeatAsync(Guid id, CancellationToken cts = default)
        {
            if (await _repository.GetAsync(id, cts) is not Event @event)
            {
                // Событие может быть удалено
                return;
            }

            // Освободить зарезервированное место
            @event.ReleaseSeats();

            await _repository.TryUpdateContextAsync(@event, cts);
        }
    }
}
