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
                using var scope = _scopeFactory.CreateScope();
                var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
                var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

                // Добавляем задержку в случае отсутствия задач, чтобы не зависало
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);

                var pendingBookings = bookingService.GetPending()
                    .ToList();

                var tasks = pendingBookings
                    // Добавляем задачи по обработки брони
                    .Select(booking => ProcessBookingAsync(bookingService, booking, eventService, stoppingToken));

                await Task.WhenAll(tasks);
            }
        }

        public async Task ProcessBookingAsync(IBookingService bookingService, Booking data, IEventService eventService, CancellationToken stoppingToken)
        {
            try
            {
                // Имитация внешнего вызова
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);

                var hasEvent = await eventService.GetEventAsync(data.EventId, stoppingToken) is not null;

                await _processingSemaphore.WaitAsync(stoppingToken);

                try
                {
                    if (hasEvent)
                    {
                        bookingService.ConfirmBooking(data);
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
                CancelOperation(bookingService, data);
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
                    CancelOperation(bookingService, data);
                }
            }
        }

        private void CancelOperation(IBookingService bookingService, Booking data)
        {
            _logger.Warn($"Booking operation was cancelled. Event Id = '{data.EventId}', Booking Id = '{data.Id}'");
            bookingService.CancelBooking(data.Id);
        }
    }
}
