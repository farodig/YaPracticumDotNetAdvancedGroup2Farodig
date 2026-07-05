using LearningWebApi.Entities;
using LearningWebApi.Exceptions;
using LearningWebApi.Repositories;

namespace LearningWebApi.Services.EventService
{
    internal class EventService(IEventRepository repository) : IEventService
    {
        private readonly IEventRepository _repository = repository;

        public IQueryable<Event> GetEvents()
        {
            return _repository.GetEvents();
        }

        public async Task<Event?> GetEventAsync(Guid id, CancellationToken? cts = null)
        {
            return await _repository.GetAsync(id, cts ?? CancellationToken.None);
        }

        public async Task<Event> CreateEventAsync(string title, DateTime startAt, DateTime endAt, int totalSeats, string? description = null, CancellationToken? cts = null)
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

            await _repository.CreateAsync(item, cts ?? CancellationToken.None);

            return item;
        }

        public async Task<bool> TryUpdateEventAsync(Event item, CancellationToken? cts = null)
        {
            return await _repository.TryUpdateAsync(item, cts ?? CancellationToken.None) > 0;
        }

        public async Task<bool> TryDeleteEventAsync(Guid id, CancellationToken? cts = null)
        {
            return await _repository.TryRemoveAsync(id, cts ?? CancellationToken.None) > 0;
        }

        public async Task ReserveSeatAsync(Guid id, CancellationToken? cts = null)
        {
            // Получить событие из хранилища
            if (await _repository.GetAsync(id, cts ?? CancellationToken.None) is not Event @event) throw new EventNotFoundException();

            // Попытка зарезервировать свободное место
            if (!@event.TryReserveSeats()) throw new NoAvailableSeatsException();

            await _repository.TryUpdateContextAsync(@event, cts ?? CancellationToken.None);
        }

        public async Task ReleaseSeatAsync(Guid id, CancellationToken? cts = null)
        {
            if (await _repository.GetAsync(id, cts ?? CancellationToken.None) is not Event @event)
            {
                // Событие может быть удалено
                return;
            }

            // Освободить зарезервированное место
            @event.ReleaseSeats();

            await _repository.TryUpdateContextAsync(@event, cts ?? CancellationToken.None);
        }
    }
}
