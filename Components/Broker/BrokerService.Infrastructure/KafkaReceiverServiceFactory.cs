using BrokerService.Application;
using Microsoft.Extensions.Options;

namespace BrokerService.Infrastructure
{
    /// <summary>
    /// Фабрика создания сервиса получения событий
    /// </summary>
    public class KafkaReceiverServiceFactory(IOptions<KafkaSettings> options) : IReceiverServiceFactory
    {
        private readonly IOptions<KafkaSettings> _options = options;

        IReceiveService<TEvent> IReceiverServiceFactory.CreateReceiverService<TEvent>()
        {
            return new KafkaReceiver<TEvent>(_options);
        }
    }
}
