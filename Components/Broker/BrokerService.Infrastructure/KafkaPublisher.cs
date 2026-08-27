using BrokerService.Application;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedContracts.Abstractions;
using System.Text.Json;

namespace BrokerService.Infrastructure
{
    public class KafkaPublisher : IPublishService, IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<KafkaPublisher> _logger;

        public KafkaPublisher(IOptions<KafkaSettings> settings, ILogger<KafkaPublisher> logger)
        {
            _logger = logger;

            var kafkaSettings = settings.Value;

            var config = new ProducerConfig
            {
                BootstrapServers = kafkaSettings.BootstrapServers,
                Acks = Acks.All
            };

            _producer = new ProducerBuilder<string, string>(config)
                .SetValueSerializer(Serializers.Utf8)
                .Build();
        }

        public async Task PublishAsync<TEvent>(TEvent data, CancellationToken ct = default)
            where TEvent : class, IEvent
        {
            var message = new Message<string, string>
            {
                Key = data.Id.ToString(),
                Value = JsonSerializer.Serialize(data),
            };

            var result = await _producer.ProduceAsync(typeof(TEvent).Name, message, ct);
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace("Sent {type} {Offset} {Partition}", typeof(TEvent).Name, result.Offset, result.Partition);
            }
        }

        #region IDisposable
        public void Dispose()
        {
            _producer.Flush(TimeSpan.FromSeconds(10));
            _producer.Dispose();
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable
    }
}
