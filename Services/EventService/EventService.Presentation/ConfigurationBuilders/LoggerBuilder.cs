using NLog.Web;

namespace EventService.Presentation.ConfigurationBuilders
{
    internal static class LoggerBuilder
    {
        /// <summary>
        /// Добавить Nlog
        /// </summary>
        public static void AddNlog(this WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();
        }
    }
}
