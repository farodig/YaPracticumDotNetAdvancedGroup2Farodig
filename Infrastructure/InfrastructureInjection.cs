using Application.Repositories;
using Infrastructure.DataAccess;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class InfrastructureInjection
    {
        /// <summary>
        /// Добавить сервис событий
        /// </summary>
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
        }

        public static void AddInrfastructureDB(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
        }
    }
}
