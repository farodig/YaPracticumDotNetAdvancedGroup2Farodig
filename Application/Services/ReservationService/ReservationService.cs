using Application.Abstractions;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Services.ReservationService
{
    public class ReservationService(IEventRepository eventRepository, IBookingRepository bookingRepository) : IReservationService
    {
        private readonly IEventRepository _eventRepository = eventRepository;
        private readonly IBookingRepository _bookingRepository = bookingRepository;

        public async Task ReserveSeatAsync(Guid eventId, Guid personId, CancellationToken cts = default)
        {
            // Получить событие из хранилища
            if (await _eventRepository.GetAsync(eventId, cts) is not Event @event) throw new EventNotFoundException();

            // Запрет на бронирование события, которое уже началось
            if (@event.StartAt <= DateTime.Now) throw new PastEventBookingException();

            // Пользователь достиг лимита на количество активных броней
            if (IReservationService.PersonMaxBookingCount <= await _bookingRepository.GetBookingCountAsync(personId, cts))
                throw new ActiveBookingLimitException(limit: IReservationService.PersonMaxBookingCount);

            // Попытка зарезервировать свободное место
            if (!@event.TryReserveSeats()) throw new NoAvailableSeatsException();

            await _eventRepository.TryUpdateContextAsync(@event, cts);
        }

        public async Task ReleaseSeatAsync(Guid id, CancellationToken cts = default)
        {
            if (await _eventRepository.GetAsync(id, cts) is not Event @event)
            {
                // Событие может быть удалено
                return;
            }

            // Освободить зарезервированное место
            @event.ReleaseSeats();

            await _eventRepository.TryUpdateContextAsync(@event, cts);
        }
    }
}
