using EventService.Application.Abstractions;
using EventService.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PublishService.Application;
using SharedContracts.Events;

namespace EventService.Application.EventProcessors
{
    public class BookingFailureEventProcessor(IReceiveService receiver, IServiceScopeFactory scopeFactory) : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly IReceiveService _receiver = receiver;

        private async Task ProcessEvent(BookingFailureEvent data, CancellationToken cts = default)
        {
            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IEventRepository>();
            await ReleaseSeatAsync(repository, data.EventId, cts);
        }

        /// <summary>
        /// Освободить место на событии
        /// </summary>
        private static async Task ReleaseSeatAsync(IEventRepository repository, Guid eventId, CancellationToken cts = default)
        {
            if (await repository.GetAsync(eventId, cts) is not Event data)
            {
                // Событие может быть удалено
                return;
            }

            // Освободить зарезервированное место
            data.ReleaseSeats();

            await repository.TryUpdateAsync(data, cts);

            // TODO: обработать ошибки
            // TODO: отправить ответ, чтобы booking смог обработать и отклонить
        }

        #region IHostedService
        public async Task StartAsync(CancellationToken cts = default)
        {
            await _receiver.StartAsync<BookingFailureEvent>(ProcessEvent, cts);
        }

        public async Task StopAsync(CancellationToken cts = default)
        {
            await _receiver.StopAsync(cts);
        }
        #endregion IHostedService
    }
}
