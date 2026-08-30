using BrokerService.Application;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedContracts.Abstractions;

namespace BrokerService.Infrastructure
{
    /// <summary>
    /// Фабрика создания сервиса получения событий
    /// </summary>
    public class KafkaReceiverServiceFactory(IOptions<KafkaSettings> options, ILogger<KafkaReceiver<IEvent>> logger) : IReceiverServiceFactory
    {
        IReceiveService<TEvent> IReceiverServiceFactory.CreateReceiverService<TEvent>()
        {
            return new KafkaReceiver<TEvent>(options, logger);
        }
    }
}
