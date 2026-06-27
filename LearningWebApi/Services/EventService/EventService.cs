using LearningWebApi.Entities;
using LearningWebApi.Exceptions;
using LearningWebApi.Repositories;

namespace LearningWebApi.Services.EventService
{
    internal class EventService(IEventRepository repository) : IEventService
    {
        private readonly IEventRepository _repository = repository;

        public IEnumerable<Event> GetEvents()
        {
            return _repository.Values;
        }

        public Event? GetEvent(Guid id)
        {
            var item = _repository.Get(id);
            return item;
        }

        public Event CreateEvent(string title, DateTime startAt, DateTime endAt, int totalSeats, string? description = null)
        {
            var id = Guid.NewGuid();

            var item = new Event()
            {
                Id = id,
                Title = title,
                Description = description,
                StartAt = startAt,
                EndAt = endAt,
                TotalSeats = totalSeats,
                AvailableSeats = totalSeats,
            };
            _repository.CreateOrUpdate(item);

            return item;
        }

        public bool TryUpdateEvent(Event item)
        {
            if (_repository.Get(item.Id) is null)
            {
                return false;
            }

            _repository.CreateOrUpdate(item);
            return true;
        }

        public bool TryDeleteEvent(Guid id)
        {
            if (!_repository.ContainsKey(id))
            {
                return false;
            }

            _repository.Remove(id);
            return true;
        }

        public void ReserveSeat(Guid id)
        {
            // Получить событие из хранилища
            if (_repository.Get(id) is not Event @event) throw new EventNotFoundException();

            // Попытка зарезервировать свободное место
            if (!@event.TryReserveSeats()) throw new NoAvailableSeatsException();

            // Сохранить в репозитории
            _repository.CreateOrUpdate(@event);
        }

        public void ReleaseSeat(Guid id)
        {
            if (_repository.Get(id) is not Event @event)
            {
                // Событие может быть удалено
                return;
            }

            // Освободить зарезервированное место
            @event.ReleaseSeats();

            // Сохранить в репозитории
            _repository.CreateOrUpdate(@event);
        }
    }
}
