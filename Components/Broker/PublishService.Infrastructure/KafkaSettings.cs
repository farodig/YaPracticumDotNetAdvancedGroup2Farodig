namespace PublishService.Infrastructure
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
    }
}
