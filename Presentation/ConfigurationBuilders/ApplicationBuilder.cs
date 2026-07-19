using Application;
using Application.Services.TokenService;
using Infrastructure;

namespace Presentation.ConfigurationBuilders
{
    internal static class ApplicationBuilder
    {
        public static void ConfigureApplication(this WebApplicationBuilder builder)
        {
            builder.Services.AddRepositories();
            builder.Services.AddApplicationServices();

            // TODO: по хорошему следует вынести в AddApplicationServices, однако это потребует подключения библиотек
            builder.Services.Configure<TokenSettings>(
                builder.Configuration.GetSection("TokenSettings"));
            builder.Services.AddOptions<TokenSettings>()
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }
    }
}
