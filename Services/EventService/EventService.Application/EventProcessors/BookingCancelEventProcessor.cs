using BrokerService.Application;
using EventService.Application.Abstractions;
using EventService.Application.Models.Builders;
using EventService.Domain.Entities;
using EventService.Domain.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using SharedContracts.Events.BookingEvents;

namespace EventService.Application.EventProcessors
{
    public class BookingCancelEventProcessor(IReceiverServiceFactory receiveFactory, IServiceScopeFactory scopeFactory) : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly IReceiveService<BookingCancelEvent> _receiver = receiveFactory.CreateReceiverService<BookingCancelEvent>();
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Освободить место на событии
        /// </summary>
        private async Task ReleaseSeatAsync(BookingCancelEvent @event, CancellationToken cts = default)
        {
            _logger.Info("Request to release seats: {@event}", @event);

            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IEventRepository>();
            var publishService = scope.ServiceProvider.GetRequiredService<IPublishService>();

            try
            {
                // Получить событие из хранилища
                if (await repository.GetAsync(@event.EventId, cts) is not Event data) throw new EventNotFoundException();

                // Освободить зарезервированное место
                data.ReleaseSeats();

                await repository.TryUpdateAsync(data, cts);
                await publishService.PublishReleaseSeatsEvent(@event, cts);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                await publishService.PublishUnableToReleaseSeatsEvent(@event, ex.Message, cts);
            }
        }

        #region IHostedService
        public Task StartAsync(CancellationToken cts = default)
        {
            _ = _receiver.StartAsync(ReleaseSeatAsync, cts);
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cts = default)
        {
            await _receiver.StopAsync(cts);
        }
        #endregion IHostedService
    }
}
