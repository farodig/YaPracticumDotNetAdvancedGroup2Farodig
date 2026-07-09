using LearningWebApi.Entities;
using LearningWebApi.Services.EventService;
using NLog;

namespace LearningWebApi.Services.BookingService
{
    internal class BookingProcessor(IServiceScopeFactory scopeFactory) : BackgroundService
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
                var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
                var hasEvent = await eventService.GetEventAsync(data.EventId, stoppingToken) is not null;

                await _processingSemaphore.WaitAsync(stoppingToken);

                try
                {
                    if (hasEvent)
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
                await bookingService.CancelBookingAsync(data, CancellationToken.None);
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
                    await bookingService.CancelBookingAsync(data, CancellationToken.None);
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
    }
}
