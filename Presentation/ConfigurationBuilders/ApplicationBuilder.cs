using Application;
using Infrastructure;

namespace Presentation.ConfigurationBuilders
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
