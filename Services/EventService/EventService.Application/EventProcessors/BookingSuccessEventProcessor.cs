using EventService.Application.Abstractions;
using EventService.Domain.Entities;
using EventService.Domain.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PublishService.Application;
using SharedContracts.Events;

namespace EventService.Application.EventProcessors
{
    public class BookingSuccessEventProcessor(IReceiveService receiver, IServiceScopeFactory scopeFactory) : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly IReceiveService _receiver = receiver;

        private async Task ProcessEvent(BookingSuccessEvent @event, CancellationToken cts = default)
        {
            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IEventRepository>();
            await ReserveSeatAsync(repository, @event.EventId, cts);
        }

        /// <summary>
        /// Зарезерировать место на событии
        /// </summary>
        private static async Task ReserveSeatAsync(IEventRepository repository, Guid eventId, CancellationToken cts = default)
        {
            // Получить событие из хранилища
            if (await repository.GetAsync(eventId, cts) is not Event data) throw new EventNotFoundException();

            // Запрет на бронирование события, которое уже началось
            if (data.StartAt <= DateTime.Now) throw new PastEventReserveException();

            // Попытка зарезервировать свободное место
            if (!data.TryReserveSeats()) throw new NoAvailableSeatsException();

            await repository.TryUpdateAsync(data, cts);

            // TODO: обработать ошибки
            // TODO: отправить ответ, чтобы booking смог обработать и подтвердить или отклонить
        }

        #region IHostedService
        public async Task StartAsync(CancellationToken cts = default)
        {
            await _receiver.StartAsync<BookingSuccessEvent>(ProcessEvent, cts);
        }

        public async Task StopAsync(CancellationToken cts = default)
        {
            await _receiver.StopAsync(cts);
        }
        #endregion IHostedService
    }
}
