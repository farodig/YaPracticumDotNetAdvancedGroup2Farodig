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
                var pendingBookings = _bookingService.GetPending()
                    .ToList();

                var tasks = pendingBookings
                    // Добавляем задачи по обработки брони
                    .Select(booking => ProcessBookingAsync(booking, stoppingToken))
                    // Добавляем задержку в случае отсутствия задач, чтобы не зависало
                    .Append(Task.Delay(TimeSpan.FromSeconds(2), stoppingToken));

                await Task.WhenAll(tasks);
            }
        }

        public async Task ProcessBookingAsync(Booking data, CancellationToken stoppingToken)
        {
            try
            {
                await _processingSemaphore.WaitAsync(stoppingToken);

                if (_eventRepository.GetEvent(data.EventId) is null)
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
            finally
            {
                _processingSemaphore.Release();
            }
        }
    }
}
