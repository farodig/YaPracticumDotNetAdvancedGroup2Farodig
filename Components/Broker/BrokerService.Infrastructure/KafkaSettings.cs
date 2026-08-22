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
        public int SessionTimeoutSeconds { get; set; } = 60;
        public int HeartbeatIntervalSeconds { get; set; } = 5;
    }
}
