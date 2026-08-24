namespace BrokerService.Infrastructure
{
    /// <summary>
    /// Настройки кафки
    /// </summary>
    public class KafkaSettings
    {
        /// <summary>
        /// Адреса серверов кафки
        /// </summary>
        public string BootstrapServers { get; set; } = "localhost:9092";
        public int SessionTimeoutMs { get; set; } = 60000;
        public int HeartbeatIntervalMs { get; set; } = 5000;
    }
}
