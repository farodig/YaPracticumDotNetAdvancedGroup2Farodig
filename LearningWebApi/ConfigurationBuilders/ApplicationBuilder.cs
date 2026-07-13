using Application;
using Infrastructure;

namespace LearningWebApi.ConfigurationBuilders
{
    internal static class ApplicationBuilder
    {
        public static void ConfigureApplication(this WebApplicationBuilder builder)
        {
            builder.Services.AddRepositories();
            builder.Services.AddEventService();
            builder.Services.AddBookingService();
        }
    }
}
