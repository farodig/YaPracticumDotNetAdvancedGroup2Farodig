using BrokerService.Application;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Options;
using SharedContracts.Abstractions;
using System.Text.Json;

namespace BrokerService.Infrastructure
{
    public class KafkaReceiver : IReceiveService
    {
        private readonly ConsumerConfig _config;
        private readonly IConsumer<string, string> _consumer;
        private readonly SemaphoreSlim _stopSemaphore = new(1, 1);
        private bool _isStarted = true;

        public KafkaReceiver(IOptions<KafkaSettings> settings)
        {
            var kafkaSettings = settings.Value;

            _config = new ConsumerConfig
            {
                BootstrapServers = kafkaSettings.BootstrapServers,
                GroupId = "TestGroupId",// Guid.NewGuid().ToString(),
                AutoOffsetReset = AutoOffsetReset.Earliest,
                SessionTimeoutMs = kafkaSettings.SessionTimeoutMs,
                HeartbeatIntervalMs = kafkaSettings.HeartbeatIntervalMs,
                EnableAutoCommit = false,
                EnablePartitionEof = false
            };

            _consumer = new ConsumerBuilder<string, string>(_config)
            .SetValueDeserializer(Deserializers.Utf8)
            .Build();
        }

        private async Task CreateTopic<TEvent>(CancellationToken cts = default)
            where TEvent : class, IEvent
        {

            // Создание топика
            using var admin = new AdminClientBuilder(new AdminClientConfig
            {
                BootstrapServers = _config.BootstrapServers
            }).Build();

            var metadata = admin.GetMetadata(TimeSpan.FromSeconds(10));
            var existingTopics = metadata.Topics.Select(t => t.Topic).ToList();

            
            if (existingTopics.Contains(typeof(TEvent).Name))
            {
                return;
            }

            await admin.CreateTopicsAsync(new[]
            {
                new TopicSpecification
                {
                    Name = typeof(TEvent).Name,
                }
            });
        }

        public async Task StartAsync<TEvent>(Func<TEvent, CancellationToken, Task> handler, CancellationToken cts = default)
            where TEvent : class, IEvent
        {
            _isStarted = true;
            await CreateTopic<TEvent>(cts);
            _consumer.Subscribe(typeof(TEvent).Name);
            try
            {
                while (_isStarted && !cts.IsCancellationRequested)
                {
                    await _stopSemaphore.WaitAsync(cts);
                    try
                    {
                        await ProcessAsync(handler, cts);
                    }
                    finally
                    {
                        _stopSemaphore.Release();
                    }
                }
            }
            finally
            {
                _consumer.Close();
            }
        }

        private async Task ProcessAsync<TEvent>(Func<TEvent, CancellationToken, Task> handler, CancellationToken cts = default)
            where TEvent : class, IEvent
        {
            var result = _consumer.Consume(TimeSpan.FromSeconds(5));

            if (result == null)
            {
                return;
            }

            try
            {
                if (JsonSerializer.Deserialize<TEvent>(result.Message.Value) is not TEvent message)
                {
                    return;
                }

                await handler(message, cts);
            }
            finally
            {
                _consumer.Commit(result);
            }
        }

        public async Task StopAsync(CancellationToken cts = default)
        {
            await _stopSemaphore.WaitAsync(cts);
            try
            {
                _isStarted = false;
            }
            finally
            {
                _stopSemaphore.Release();
            }
        }

        #region IDisposable
        public void Dispose()
        {
            _consumer.Dispose();
            _stopSemaphore.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable
    }
}
