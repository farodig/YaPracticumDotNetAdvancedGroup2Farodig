using SharedContracts.Abstractions;

namespace BrokerService.Application
{
    public interface IReceiverServiceFactory
    {
        public IReceiveService<TEvent> CreateReceiverService<TEvent>()
            where TEvent : class, IEvent;
    }
}
