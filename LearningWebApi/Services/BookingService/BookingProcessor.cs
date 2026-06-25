using LearningWebApi.Entities;
using LearningWebApi.Repositories;
using NLog;

namespace LearningWebApi.Services.BookingService
{
    internal class BookingProcessor(IBookingRepository bookingRepository, IEventRepository eventRepository) : BackgroundService
    {
        private readonly IEventRepository _eventRepository = eventRepository;
        private readonly IBookingRepository _bookingRepository = bookingRepository;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly SemaphoreSlim _processingSemaphore = new(1, 1);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var pendingBookings = GetPending()
                    .ToList();

                var tasks = pendingBookings
                    // Добавляем задачи по обработки брони
                    .Select(booking => ProcessBookingAsync(booking, stoppingToken))
                    // Добавляем задержку в случае отсутствия задач, чтобы не зависало
                    .Append(Task.Delay(TimeSpan.FromSeconds(2), stoppingToken));

                await Task.WhenAll(tasks);
            }
        }

        private IEnumerable<Booking> GetPending()
        {
            return _bookingRepository.Select(a => a.Value)
                .Where(a => a.Status == BookingStatus.Pending)
                .OrderBy(a => a.CreatedAt);
        }

        public async Task ProcessBookingAsync(Booking data, CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);

            try
            {
                await _processingSemaphore.WaitAsync(stoppingToken);

                if (!_eventRepository.TryGetValue(data.EventId, out Event? @event)
                    || @event is null)
                {
                    RejectBooking(data);
                    SaveData(data);
                    return;
                }

                try
                {
                    ConfirmBooking(data);
                    SaveData(data);
                }
                catch (Exception cef)
                {
                    _logger.Error(cef);
                    RejectBooking(data);
                    SaveData(data);

                    @event!.ReleaseSeats();
                    SaveData(@event);
                }
            }
            finally
            {
                _processingSemaphore.Release();
            }
        }


        private void ConfirmBooking(Booking data)
        {
            data.Status = BookingStatus.Confirmed;
            data.ProcessedAt = DateTime.Now;
            _logger.Info($"Booking #{data.Id} changed status to '{data.Status}'");
        }

        private void RejectBooking(Booking data)
        {
            data.Status = BookingStatus.Rejected;
            data.ProcessedAt = DateTime.Now;
            _logger.Warn($"Booking #{data.Id} changed status to '{data.Status}'");
        }

        private void SaveData(Booking data)
        {
            _bookingRepository[data.Id] = data;
            _bookingRepository.SaveData();
        }

        private void SaveData(Event data)
        {
            _eventRepository[data.Id] = data;
            _eventRepository.SaveData();
        }
    }
}
