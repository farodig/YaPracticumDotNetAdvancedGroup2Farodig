using LearningWebApi.Entities;
using LearningWebApi.Repositories;
using NLog;

namespace LearningWebApi.Services.BookingService
{
    internal class BookingProcessor(IBookingRepository bookingRepository) : BackgroundService
    {
        private readonly IBookingRepository _bookingRepository = bookingRepository;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Периодический опрос хранилища на наличие бронирований в статусе Pending;
                if (await GetNextPendingAsync(stoppingToken) is not Booking data)
                {
                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                    continue;
                }

                // обработка брони
                await ProcessBookingAsync(data, stoppingToken);

                // обновлённая бронь сохраняется в хранилище
                SaveData(data);
            }
        }

        public async ValueTask<Booking?> GetNextPendingAsync(CancellationToken stoppingToken)
        {
            return _bookingRepository.Select(a => a.Value)
                .Where(a => a.Status == BookingStatus.Pending)
                .OrderBy(a => a.CreatedAt)
                .FirstOrDefault();
        }

        public async ValueTask ProcessBookingAsync(Booking data, CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            data.Status = BookingStatus.Confirmed;
            data.ProcessedAt = DateTime.Now;
            _logger.Info($"Booking #{data.Id} changed status to '{data.Status}'");
        }

        private void SaveData(Booking data)
        {
            //_bookingRepository[data.Id] = data;
            // необязательное действие, т. к. данные хранятся в памяти и изменяя объект по ссылке он сразу же меняется в нашем фейковом репозитории
        }
    }
}
