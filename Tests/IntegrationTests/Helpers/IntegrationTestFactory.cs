using DotNet.Testcontainers.Containers;
using Infrastructure.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Helpers
{
    public class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        public readonly IDatabaseContainer _postgres = DatabaseContainerFactory.CreatePostgreSqlContainer();

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

        public async Task InitializeAsync()
        {
            await _postgres.StartAsync();
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await _postgres.DisposeAsync();
        }

        private AppDbContext CreateContextInternal()
        {
            var _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(_postgres.GetConnectionString())
                .Options;
            var context = new AppDbContext(_options!);
            context.Database.Migrate();
            return context;
        }

        public void ClearDatabase()
        {
            using var context = CreateContextInternal();
            context.Database.ExecuteSqlRaw(
                "TRUNCATE TABLE bookings, events, persons RESTART IDENTITY CASCADE");
            context.Dispose();
        }
    }
}
