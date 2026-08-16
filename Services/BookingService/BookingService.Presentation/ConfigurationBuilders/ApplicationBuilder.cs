using BookingService.Application;
using BookingService.Infrastructure;

namespace BookingService.Presentation.ConfigurationBuilders
{
    internal static class ApplicationBuilder
    {
        public static void ConfigureApplication(this WebApplicationBuilder builder)
        {
            builder.Services.AddExternalServices();
            builder.Services.AddRepositories();
            builder.Services.AddApplicationServices();
        }
    }
}
