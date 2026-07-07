using LearningWebApi.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LearningTest.Helpers
{
    public class IntegrationTestFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                // Удаляем все регистрации DbContext
                RemoveAllDbContextRegistrations(services);

                // Подставляем строку от контейнера
                services.AppDbInMemoryConfigure();
            });
        }

        /// <summary>
        /// Удаляем все регистрации DbContext
        /// </summary>
        private static void RemoveAllDbContextRegistrations(IServiceCollection services)
        {
            // Удаляем
            var optionsDescriptors = services
                // Сам контекст
                .Where(d => d.ServiceType == typeof(AppDbContext)||
                            // DbContextOptions
                            d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                            d.ServiceType == typeof(DbContextOptions) ||
                            // Все фабрики и провайдеры
                            d.ServiceType.Name.Contains("DbContext") ||
                            d.ServiceType.Name.Contains("DatabaseProvider"))
                .ToList();

            foreach (var descriptor in optionsDescriptors)
            {
                services.Remove(descriptor);
            }
        }
    }
}
