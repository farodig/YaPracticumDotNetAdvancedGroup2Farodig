using Application.Repositories;
using Application.Services.EventService;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Services.ReservationService
{
    public class ReservationService(IEventRepository repository) : IReservationService
    {
        private readonly IEventRepository _repository = repository;

        public async Task ReserveSeatAsync(Guid id, CancellationToken cts = default)
        {
            // Получить событие из хранилища
            if (await _repository.GetAsync(id, cts) is not Event @event) throw new EventNotFoundException();

            // Запрет на бронирование события, которое уже началось
            if (@event.StartAt <= DateTime.Now) throw new PastEventBookingException();

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
