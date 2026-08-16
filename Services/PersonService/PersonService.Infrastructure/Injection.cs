using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PersonService.Application;
using PersonService.Infrastructure.DataAccess;
using PersonService.Infrastructure.Repositories;

namespace PersonService.Infrastructure
{
    public static class Injection
    {
        /// <summary>
        /// Добавить сервис событий
        /// </summary>
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IPersonRepository, PersonRepository>();
        }

        public static void AddInrfastructureDB(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<PersonDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
        }

        public static void InitializeInfrastructure(this IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PersonDbContext>();

            context.Database.Migrate();
        }
    }
}
