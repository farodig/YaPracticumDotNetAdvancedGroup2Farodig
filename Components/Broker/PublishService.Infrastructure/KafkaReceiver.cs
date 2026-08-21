using Confluent.Kafka;
using Microsoft.Extensions.Options;
using PublishService.Application;
using SharedContracts.Abstractions;
using System.Text.Json;

namespace PublishService.Infrastructure
{
    public class KafkaReceiver : IReceiveService, IDisposable
    {
        private readonly IConsumer<string, string> _consumer;

        public KafkaReceiver(IOptions<KafkaSettings> settings)
        {
            var kafkaSettings = settings.Value;

            var config = new ConsumerConfig
            {
                BootstrapServers = kafkaSettings.BootstrapServers,
                GroupId = GetType().Name,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                SessionTimeoutMs = kafkaSettings.SessionTimeoutSeconds,
                HeartbeatIntervalMs = kafkaSettings.HeartbeatIntervalSeconds,
                EnableAutoCommit = false,
                EnablePartitionEof = false
            };

            _consumer = new ConsumerBuilder<string, string>(config)
            .SetValueDeserializer(Deserializers.Utf8)
            .Build();
        }

        public async Task StartAsync<TEvent>(Func<TEvent, Task> handler, CancellationToken cts)
            where TEvent : class, IEvent
        {
            _consumer.Subscribe(nameof(TEvent));
            try
            {
                while (!cts.IsCancellationRequested)
                {
                    var result = _consumer.Consume(TimeSpan.FromSeconds(5));

                    if (result == null)
                    {
                        continue;
                    }

                    try
                    {
                        if (JsonSerializer.Deserialize<TEvent>(result.Message.Value) is not TEvent message)
                        {
                            continue;
                        }

                        await handler(message);
                    }
                    finally
                    {
                        _consumer.Commit(result);
                    }
                }
            }
            finally
            {
                _consumer.Close();
            }
        }

        #region IDisposable
        public void Dispose()
        {
            _consumer.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable
    }
}
