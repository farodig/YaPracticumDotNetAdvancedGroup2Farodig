using Confluent.Kafka;
using Microsoft.Extensions.Options;
using PublishService.Application;
using System.Text.Json;

namespace PublishService.Infrastructure
{
    public class KafkaPublisher : IPublishService, IDisposable
    {
        private readonly IProducer<string, string> _producer;

        public KafkaPublisher(IOptions<KafkaSettings> settings)
        {
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

        public async Task PublishAsync<TEvent>(string topic, string key, TEvent message, CancellationToken ct = default)
            where TEvent : class
        {
            var kafkaMessage = new Message<string, string>
            {
                Key = key,
                Value = JsonSerializer.Serialize(message),
            };

            await _producer.ProduceAsync(topic, kafkaMessage, ct);
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
