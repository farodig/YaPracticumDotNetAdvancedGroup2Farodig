using LearningWebApi.Entities;
using LearningWebApi.Repositories;
using NLog;

namespace LearningWebApi.Services.BookingService
{
    internal class BookingProcessor(IBookingService bookingService, IEventRepository eventRepository) : BackgroundService
    {
        private readonly IEventRepository _eventRepository = eventRepository;
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
                await _processingSemaphore.WaitAsync(stoppingToken);

                if (_eventRepository.Get(data.EventId) is null)
                {
                    _bookingService.RejectBooking(data);
                    return;
                }

                try
                {
                    _bookingService.ConfirmBooking(data);
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
