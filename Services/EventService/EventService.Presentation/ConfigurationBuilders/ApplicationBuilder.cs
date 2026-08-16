using EventService.Application;
using EventService.Infrastructure;

namespace EventService.Presentation.ConfigurationBuilders
{
    internal static class ApplicationBuilder
    {
        public static void ConfigureApplication(this WebApplicationBuilder builder)
        {
            builder.Services.AddRepositories();
            builder.Services.AddApplicationServices();
        }
    }
}
