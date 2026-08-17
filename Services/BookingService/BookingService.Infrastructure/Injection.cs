using BookingService.Application.Abstractions;
using BookingService.Infrastructure.DataAccess;
using BookingService.Infrastructure.Repositories;
using KafkaBrokerService;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TokenService;

namespace BookingService.Infrastructure
{
    public static class Injection
    {
        public static void AddExternalServices(this WebApplicationBuilder builder)
        {
            builder.AddBroker();
            builder.Services.AddSingleton<ITokenService, JwtTokenService>();
        }

        /// <summary>
        /// Добавить сервис событий
        /// </summary>
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IBookingRepository, BookingRepository>();
        }

        public static void AddInrfastructureDB(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<BookingDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
        }

        public static void InitializeInfrastructure(this IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BookingDbContext>();

            context.Database.Migrate();
        }
    }
}
