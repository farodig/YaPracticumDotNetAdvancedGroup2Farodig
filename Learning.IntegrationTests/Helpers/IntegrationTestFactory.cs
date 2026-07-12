using DotNet.Testcontainers.Containers;
using LearningWebApi.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static Learning.IntegrationTests.Helpers.DatabaseContainerFactory;

namespace Learning.IntegrationTests.Helpers
{
    public class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly IDatabaseContainer _postgres = CreateTestPostgeSqlContaner();//CreatePostgreSqlContainer();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                // Удаляем все регистрации DbContext
                RemoveAllDbContextRegistrations(services);

                // Подставляем строку от контейнера
                services.AddDbContext<AppDbContext>(options => 
                    options.UseNpgsql(_postgres.GetConnectionString()));
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

        #region IAsyncLifetime
        public async Task InitializeAsync()
        {
            await _postgres.StartAsync();
        }

        new public async Task DisposeAsync()
        {
            await _postgres.DisposeAsync();
            await base.DisposeAsync();
        }
        #endregion IAsyncLifetime
    }
}
