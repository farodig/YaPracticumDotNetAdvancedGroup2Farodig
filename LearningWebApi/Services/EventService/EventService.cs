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
            var item = _repository.GetEvent(id);
            return item;
        }

        public Event CreateEvent(string title, DateTime startAt, DateTime endAt, int totalSeats, string? description = null)
        {
            var id = Guid.NewGuid();

            return _repository[id] = new Event()
            {
                Id = id,
                Title = title,
                Description = description,
                StartAt = startAt,
                EndAt = endAt,
                TotalSeats = totalSeats,
                AvailableSeats = totalSeats,
            };
        }

        public bool TryUpdateEvent(Event item)
        {
            if (_repository.GetEvent(item.Id) is not Event oldValue)
            {
                return false;
            }

            // Очевидный костыль если при обновлении не передали значение значит не меняем,
            // в ТЗ отсутствует информация об обнолении но чётко уазано что AvailableSeats опциональный,
            // а так же в ТЗ на этапе 2 опустились до названий объектов,
            // которые по очевидным причинам у меня отличаются поэтому задание можно не однозначно понять
            item.AvailableSeats ??= oldValue.AvailableSeats;

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

        public void ReserveSeat(Guid id)
        {
            // Получить событие из хранилища
            if (_repository.GetEvent(id) is not Event @event) throw new EventNotFoundException();

            // Проверить на наличие свободных незарегистрированных мест
            if (!@event.TryReserveSeats()) throw new NoAvailableSeatsException();

            // Обновляем событие в репозитории TODO: сейчас в этом нет смысла, т. к. хранится в памяти, а в будущем контракт метода изменится)
            _repository.TryUpdate(id, @event, @event);
        }

        public void ReleaseSeat(Guid id)
        {
            if (_repository.GetEvent(id) is not Event @event)
            {
                // Событие может быть удалено
                return;
            }

            @event.ReleaseSeats();
            _repository[id] = @event;
        }
    }
}
