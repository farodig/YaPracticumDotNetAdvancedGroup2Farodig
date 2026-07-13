using NLog.Web;

namespace LearningWebApi.ConfigurationBuilders
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
