using BrokerService.Application;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Options;
using NLog;
using SharedContracts.Abstractions;
using System.Text.Json;

namespace BrokerService.Infrastructure
{
    public class KafkaReceiver<TEvent> : IReceiveService<TEvent>
        where TEvent : class, IEvent
    {
        private readonly ConsumerConfig _config;
        private readonly IConsumer<string, string> _consumer;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private bool _isStarted = true;

        public KafkaReceiver(IOptions<KafkaSettings> settings)
        {
            var kafkaSettings = settings.Value;

            _config = new ConsumerConfig
            {
                BootstrapServers = kafkaSettings.BootstrapServers,
                GroupId = typeof(TEvent).Name,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                SessionTimeoutMs = kafkaSettings.SessionTimeoutMs,
                HeartbeatIntervalMs = kafkaSettings.HeartbeatIntervalMs,
                EnableAutoCommit = false,
                EnablePartitionEof = false,
            };

            _consumer = new ConsumerBuilder<string, string>(_config)
            .SetValueDeserializer(Deserializers.Utf8)
            .Build();
        }

        private async Task CreateTopicIfNotExist(CancellationToken cts = default)
        {
            using var admin = new AdminClientBuilder(new AdminClientConfig
            {
                BootstrapServers = _config.BootstrapServers,
            }).Build();

            var metadata = admin.GetMetadata(TimeSpan.FromSeconds(10));

            if (metadata.Topics.FirstOrDefault(a => a.Topic.Equals(typeof(TEvent).Name)) is TopicMetadata topic)
            {
                //await admin.DeleteTopicsAsync(new[] { typeof(TEvent).Name });
                return;
            }

            await admin.CreateTopicsAsync(new[]
            {
                new TopicSpecification
                {
                    Name = typeof(TEvent).Name,
                    NumPartitions = 1,
                    ReplicationFactor = 1,
                }
            });

            _logger.Info($"New topic created <{typeof(TEvent).Name}>");
        }

        public Task StartAsync(Func<TEvent, CancellationToken, Task> handler, CancellationToken cts = default)
        {
            _isStarted = true;
            return Task.Run(async () => await DoWorkAsync(handler, cts), cts);
        }

        private async Task DoWorkAsync(Func<TEvent, CancellationToken, Task> handler, CancellationToken cts = default)
        {
            await CreateTopicIfNotExist(cts);
            _consumer.Subscribe(typeof(TEvent).Name);

            try
            {
                while (_isStarted && !cts.IsCancellationRequested)
                {
                    await ProcessAsync(handler, cts);
                }
            }
            finally
            {
                _consumer.Close();
            }
        }

        private async Task ProcessAsync(Func<TEvent, CancellationToken, Task> handler, CancellationToken cts = default)
        {
            try
            {
                var result = _consumer.Consume(TimeSpan.FromSeconds(1));

                if (result == null)
                {
                    return;
                }

                try
                {
                    _logger.Trace($"Received<{typeof(TEvent).Name}>: offset={result.Offset}, partition={result.Partition}");
                    if (JsonSerializer.Deserialize<TEvent>(result.Message.Value) is not TEvent message)
                    {
                        _logger.Error($"Unable to deserialize {typeof(TEvent).Name}");
                        return;
                    }

                    await handler(message, cts);
                }
                finally
                {
                    _consumer.Commit(result);
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public async Task StopAsync(CancellationToken cts = default)
        {
            _isStarted = false;
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
