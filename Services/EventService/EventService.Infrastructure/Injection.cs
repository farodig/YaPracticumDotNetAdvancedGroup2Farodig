using BrokerService.Infrastructure;
using EventService.Application.Abstractions;
using EventService.Infrastructure.DataAccess;
using EventService.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TokenService;

namespace EventService.Infrastructure
{
    public static class Injection
    {
        public static void AddExternalServices(this WebApplicationBuilder builder)
        {
            builder.AddPublishService();
            builder.AddReceiveFactory();
            builder.Services.AddSingleton<ITokenService, JwtTokenService>();
        }

        /// <summary>
        /// Добавить сервис событий
        /// </summary>
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IEventRepository, EventRepository>();
        }

        public static void AddInrfastructureDB(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<EventDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
        }

        public static void InitializeInfrastructure(this IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<EventDbContext>();

            context.Database.Migrate();
        }
    }
}
