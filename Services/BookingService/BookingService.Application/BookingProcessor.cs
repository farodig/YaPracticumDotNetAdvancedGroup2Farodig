using BookingService.Application.Abstractions;
using BookingService.Domain.Entities;
using BrokerService.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharedContracts.Events.EventEvents;

namespace BookingService.Application
{
    public class BookingProcessor(IReceiverServiceFactory receiveFactory, IServiceScopeFactory scopeFactory, ILogger<BookingProcessor> logger) : BackgroundService
    {
        private readonly IReceiveService<ReserveSeatsEvent> _successReceiver = receiveFactory.CreateReceiverService<ReserveSeatsEvent>();
        private readonly IReceiveService<UnableToReserveSeatsEvent> _failureReceiver = receiveFactory.CreateReceiverService<UnableToReserveSeatsEvent>();
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly ILogger<BookingProcessor> _logger = logger;
        private readonly SemaphoreSlim _processingSemaphore = new(1, 1);
        private readonly Dictionary<Guid, bool> seatsReservedForBooking = [];

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _ =_successReceiver.StartAsync(OnSuccessReserved, stoppingToken);
            _ = _failureReceiver.StartAsync(OnFailureReserved, stoppingToken);

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
                    // Ответ от сервиса обработан
                    if (!seatsReservedForBooking.Remove(data.Id, out bool isSeatsReserved))
                    {
                        // Не получили ответ от сервиса событий - пока рано обрабатывать бронирование
                        return;
                    }

                    // Проверка что места забронированы
                    if (isSeatsReserved)
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
                    _logger.LogError(cef, "Unable to process booking {BookingId}, {EventId}", data.Id, data.EventId);
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogCritical(ex, "BookingProcessor process was cancelled");
            }
            finally
            {
                try
                {
                    _processingSemaphore.Release();
                }
                catch (SemaphoreFullException ex)
                {
                    _logger.LogCritical(ex, "BookingProcessor process was interrupted before WaitAsync");
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

        /// <summary>
        /// Получили ответ от сервиса событий - успешно выделены места
        /// </summary>
        private async Task OnSuccessReserved(ReserveSeatsEvent @event, CancellationToken token)
        {
            seatsReservedForBooking[@event.BookingId] = true;
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Seats reserved: {event}", @event);
            }
        }

        /// <summary>
        /// Получили ответ от сервиса событий - места не забронированы
        /// </summary>
        private async Task OnFailureReserved(UnableToReserveSeatsEvent @event, CancellationToken token)
        {
            seatsReservedForBooking[@event.BookingId] = false;
            _logger.LogWarning("Seats not reserved: {event}", @event);
        }
    }
}
