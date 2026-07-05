using Microsoft.EntityFrameworkCore;

namespace LearningWebApi.DataAccess
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

            if (app.Environment.IsProduction())
            {
                context.Database.Migrate();
            }
            else if (app.Environment.IsDevelopment())
            {
                context.Database.EnsureCreated();
            }
        }
    }
}
