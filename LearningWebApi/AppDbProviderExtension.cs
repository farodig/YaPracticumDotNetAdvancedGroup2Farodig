using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace LearningWebApi
{
    internal static class AppDbProviderExtension
    {
        /// <summary>
        /// Конфигурирование бд
        /// </summary>
        public static void AppDbConfigure(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
        }

        /// <summary>
        /// Инициализация БД
        /// </summary>
        public static void AppDbInitialize(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Database.Migrate();
        }
    }
}
