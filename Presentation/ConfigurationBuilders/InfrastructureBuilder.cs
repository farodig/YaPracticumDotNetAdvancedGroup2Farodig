using Infrastructure;

namespace Presentation.ConfigurationBuilders
{
    internal static class InfrastructureBuilder
    {
        /// <summary>
        /// Конфигурирование бд
        /// </summary>
        public static void ConfigureInfrastructure(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddInrfastructureDB(connectionString);
        }

        public static void InitializeInfrastructure(this WebApplication app)
        {
            app.Services.InitializeInfrastructure();
        }
    }
}
