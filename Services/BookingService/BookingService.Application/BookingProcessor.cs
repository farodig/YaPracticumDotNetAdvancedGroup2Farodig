using BookingService.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using Microsoft.Extensions.Hosting;
using BookingService.Application.Abstractions;

namespace BookingService.Application
{
    public class BookingProcessor(IServiceScopeFactory scopeFactory) : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly SemaphoreSlim _processingSemaphore = new(1, 1);

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
                    if (IsSeatsReleased(data))
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

        private bool IsSeatsReleased(Booking data)
        {
            // TODO: здесь нужно узнать пришло ли сообщение из брокера о том что было выделено количество мест для бронирования
            return true;
        }

        private async Task<IEnumerable<Task>> GetBookingTasksAsync(CancellationToken cts = default)
        {
            using var scope = _scopeFactory.CreateScope();
            var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
            var orderedPendings = await bookingService.GetPendingByCreatedAsync(cts);
            return orderedPendings.Select(booking => ProcessBookingAsync(booking, cts));
        }
    }
}
