using BrokerService.Application;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using SharedContracts.Abstractions;
using System.Text.Json;

namespace BrokerService.Infrastructure
{
    public class KafkaReceiver : IReceiveService
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly SemaphoreSlim _stopSemaphore = new(1, 1);
        private bool _isStarted = true;

        public KafkaReceiver(IOptions<KafkaSettings> settings)
        {
            var kafkaSettings = settings.Value;

            var config = new ConsumerConfig
            {
                BootstrapServers = kafkaSettings.BootstrapServers,
                GroupId = Guid.NewGuid().ToString(),
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

        public async Task StartAsync<TEvent>(Func<TEvent, CancellationToken, Task> handler, CancellationToken cts = default)
            where TEvent : class, IEvent
        {
            _isStarted = true;
            _consumer.Subscribe(nameof(TEvent));
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
