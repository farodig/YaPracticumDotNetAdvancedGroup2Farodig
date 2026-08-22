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
    public class BookingSuccessEventProcessor : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IReceiveService _receiver;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public BookingSuccessEventProcessor(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            var scope = _scopeFactory.CreateScope();
            _receiver = scope.ServiceProvider.GetRequiredService<IReceiveService>();
        }

        /// <summary>
        /// Зарезерировать место на событии
        /// </summary>
        private async Task ReserveSeatAsync(BookingCreatedEvent @event, CancellationToken cts = default)
        {
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
                await publishService.PublishAsync(@event.BuildUnableToChangeSeatsEvent(ex.Message), cts); // а если не удалось забронировать так может и отменять не - нужно учитывать в рассчётах
            }
        }

        #region IHostedService
        public async Task StartAsync(CancellationToken cts = default)
        {
            await _receiver.StartAsync<BookingCreatedEvent>(ReserveSeatAsync, cts);
        }

        public async Task StopAsync(CancellationToken cts = default)
        {
            await _receiver.StopAsync(cts);
        }
        #endregion IHostedService
    }
}
