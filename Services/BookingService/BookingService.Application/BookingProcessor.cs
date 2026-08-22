using BookingService.Application.Abstractions;
using BookingService.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using PublishService.Application;
using SharedContracts.Events;

namespace BookingService.Application
{
    public class BookingProcessor(IReceiveService receiver, IServiceScopeFactory scopeFactory) : BackgroundService
    {
        private readonly IReceiveService _receiver = receiver;
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly SemaphoreSlim _processingSemaphore = new(1, 1);
        private readonly HashSet<Guid> seatsReservedForBooking = [];

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Добавляем задержку в случае отсутствия задач, чтобы не зависало
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);

                // Добавляем задачи по обработки брони
                var tasks = await GetBookingTasksAsync(stoppingToken);

                await Task.WhenAll(tasks);
            }
        }

        public async Task ProcessBookingAsync(Booking data, CancellationToken stoppingToken)
        {
            try
            {
                // Имитация внешнего вызова
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);

                using var scope = _scopeFactory.CreateScope();
                var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

                await _processingSemaphore.WaitAsync(stoppingToken);

                try
                {
                    if (IsSeatsReserved(data))
                    {
                        await bookingService.ConfirmBookingAsync(data, stoppingToken);
                    }
                    else
                    {
                        await bookingService.RejectBookingAsync(data, stoppingToken);
                    }
                }
                catch (Exception cef)
                {
                    _logger.Error(cef);
                    await bookingService.RejectBookingAsync(data, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                using var scope = _scopeFactory.CreateScope();
                var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                await bookingService.CancelBookingWhenServiceStoppedAsync(data, CancellationToken.None);
            }
            finally
            {
                try
                {
                    _processingSemaphore.Release();
                }
                // Операция была прервана раньше чем был вызыван WaitAsync
                catch (SemaphoreFullException)
                {
                    using var scope = _scopeFactory.CreateScope();
                    var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                    await bookingService.CancelBookingWhenServiceStoppedAsync(data, CancellationToken.None);
                }
            }
        }

        private async Task<IEnumerable<Task>> GetBookingTasksAsync(CancellationToken cts = default)
        {
            using var scope = _scopeFactory.CreateScope();
            var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
            var orderedPendings = await bookingService.GetPendingByCreatedAsync(cts);
            return orderedPendings.Select(booking => ProcessBookingAsync(booking, cts));
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _receiver.StartAsync<ReserveSeatsEvent>(OnReserved, cancellationToken);
            await base.StartAsync(cancellationToken);
        }

        private async Task OnReserved(ReserveSeatsEvent @event, CancellationToken token)
        {
            seatsReservedForBooking.Add(@event.Id);
        }

        private bool IsSeatsReserved(Booking data)
        {
            return seatsReservedForBooking.Remove(data.Id);
        }
    }
}
