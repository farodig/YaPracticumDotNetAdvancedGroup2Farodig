using LearningWebApi.Entities;
using LearningWebApi.Services.EventService;
using NLog;

namespace LearningWebApi.Services.BookingService
{
    internal class BookingProcessor(IBookingService bookingService, IEventService eventService) : BackgroundService
    {
        private readonly IEventService _eventService = eventService;
        private readonly IBookingService _bookingService = bookingService;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly SemaphoreSlim _processingSemaphore = new(1, 1);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Добавляем задержку в случае отсутствия задач, чтобы не зависало
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);

                var pendingBookings = _bookingService.GetPending()
                    .ToList();

                var tasks = pendingBookings
                    // Добавляем задачи по обработки брони
                    .Select(booking => ProcessBookingAsync(booking, stoppingToken));

                await Task.WhenAll(tasks);
            }
        }

        public async Task ProcessBookingAsync(Booking data, CancellationToken stoppingToken)
        {
            try
            {
                // Имитация внешнего вызова
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);

                var hasEvent = _eventService.GetEvent(data.EventId) is not null;

                await _processingSemaphore.WaitAsync(stoppingToken);

                try
                {
                    if (hasEvent)
                    {
                        _bookingService.ConfirmBooking(data);
                    }
                    else
                    {
                        _bookingService.RejectBooking(data);
                    }
                }
                catch (Exception cef)
                {
                    _logger.Error(cef);
                    _bookingService.RejectBooking(data);
                }
            }
            catch (OperationCanceledException)
            {
                CancelOperation(data);
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
                    CancelOperation(data);
                }
            }
        }

        private void CancelOperation(Booking data)
        {
            _logger.Warn($"Booking operation was cancelled. Event Id = '{data.EventId}', Booking Id = '{data.Id}'");
            _bookingService.CancelBooking(data.Id);
        }
    }
}
