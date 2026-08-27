using BrokerService.Application;
using EventService.Application.Abstractions;
using EventService.Application.Models.Builders;
using EventService.Domain.Entities;
using EventService.Domain.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharedContracts.Events.BookingEvents;

namespace EventService.Application.EventProcessors
{
    public class BookingSuccessEventProcessor(IReceiverServiceFactory receiveFactory, IServiceScopeFactory scopeFactory, ILogger<BookingSuccessEventProcessor> logger) : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly IReceiveService<BookingCreatedEvent> _receiver = receiveFactory.CreateReceiverService<BookingCreatedEvent>();
        private readonly ILogger<BookingSuccessEventProcessor> _logger = logger;

        /// <summary>
        /// Зарезерировать место на событии
        /// </summary>
        private async Task ReserveSeatAsync(BookingCreatedEvent @event, CancellationToken cts = default)
        {
            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IEventRepository>();
            var publishService = scope.ServiceProvider.GetRequiredService<IPublishService>();

            if (await repository.IsInboxDublicatedEvent(@event.Id, cts))
            {
                return;
            }

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Request to reserve seats: {@event}", @event);
            }

            try
            {
                // Получить событие из хранилища
                if (await repository.GetAsync(@event.EventId, cts) is not Event data) throw new EventNotFoundException();

                // Запрет на бронирование события, которое уже началось
                if (data.StartAt <= DateTime.Now) throw new PastEventReserveException();

                // Попытка зарезервировать свободное место
                if (!data.TryReserveSeats()) throw new NoAvailableSeatsException();

                await repository.TryUpdateAsync(data, cts);
                await publishService.PublishReserveSeatsEvent(@event, cts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to reserve seats");
                await publishService.PublishUnableToReserveSeatsEvent(@event, ex.Message, cts);
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
