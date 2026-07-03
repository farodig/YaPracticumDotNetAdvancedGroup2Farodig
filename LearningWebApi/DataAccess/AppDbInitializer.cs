using Microsoft.EntityFrameworkCore;

namespace LearningWebApi.DataAccess
{
    internal static class AppDbInitializer
    {
        /// <summary>
        /// Конфигурирование бд
        /// </summary>
        public static void AppDbConfigure(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'Default' not found.");

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
            if (app.Environment.IsDevelopment())
            {
                using var scope = app.Services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        }
    }
}
