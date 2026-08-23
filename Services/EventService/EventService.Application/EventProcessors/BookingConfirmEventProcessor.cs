using BrokerService.Application;
using EventService.Application.Abstractions;
using EventService.Application.Models.Builders;
using EventService.Domain.Entities;
using EventService.Domain.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using SharedContracts.Events;

namespace EventService.Application.EventProcessors
{
    public class BookingSuccessEventProcessor(IReceiverServiceFactory receiveFactory, IServiceScopeFactory scopeFactory) : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly IReceiveService<BookingCreatedEvent> _receiver = receiveFactory.CreateReceiverService<BookingCreatedEvent>();
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Зарезерировать место на событии
        /// </summary>
        private async Task ReserveSeatAsync(BookingCreatedEvent @event, CancellationToken cts = default)
        {
            _logger.Info("Request to reserve seats: {@event}", @event);

            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IEventRepository>();
            var publishService = scope.ServiceProvider.GetRequiredService<IPublishService>();

            try
            {
                // Получить событие из хранилища
                if (await repository.GetAsync(@event.EventId, cts) is not Event data) throw new EventNotFoundException();

                // Запрет на бронирование события, которое уже началось
                if (data.StartAt <= DateTime.Now) throw new PastEventReserveException();

                // Попытка зарезервировать свободное место
                if (!data.TryReserveSeats()) throw new NoAvailableSeatsException();

                await repository.TryUpdateAsync(data, cts);
                await publishService.PublishAsync(@event.BuildReserveSeatsEvent(), cts);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                //await publishService.PublishAsync(@event.BuildUnableToChangeSeatsEvent(ex.Message), cts); // а если не удалось забронировать так может и отменять не - нужно учитывать в рассчётах
            }
        }

        #region IHostedService
        public Task StartAsync(CancellationToken cts = default)
        {
            _ = _receiver.StartAsync(ReserveSeatAsync, cts);
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cts = default)
        {
            await _receiver.StopAsync(cts);
        }
        #endregion IHostedService
    }
}
