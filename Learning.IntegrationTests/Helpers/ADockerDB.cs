using DotNet.Testcontainers.Containers;
using LearningWebApi.DataAccess;
using Microsoft.EntityFrameworkCore;
using static Learning.IntegrationTests.Helpers.DatabaseContainerFactory;

namespace Learning.IntegrationTests.Helpers
{
    public abstract class ADockerDB : IAsyncLifetime
    {
        private readonly IDatabaseContainer _postgres = CreateTestPostgeSqlContaner();//CreatePostgreSqlContainer();

        private DbContextOptions<AppDbContext>? _options;

        internal AppDbContext CreateContext()
        {
            return CreateContextInternal();
        }

        #region IAsyncLifetime
        public async Task InitializeAsync()
        {
            await _postgres.StartAsync();
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(_postgres.GetConnectionString())
                .Options;

            // При каждом запуске теста очищаем базу
            await ClearDatabaseAsync();
        }

        public async Task DisposeAsync()
        {
            await ClearDatabaseAsync();
            await _postgres.DisposeAsync();
        }
        #endregion IAsyncLifetime

        #region Вспомогательные методы
        private AppDbContext CreateContextInternal()
        {
            var context = new AppDbContext(_options!);
            context.Database.Migrate();
            return context;
        }

        private async Task ClearDatabaseAsync()
        {
            await using var context = CreateContextInternal();
            await context.Database.ExecuteSqlRawAsync(
                "TRUNCATE TABLE bookings, events RESTART IDENTITY CASCADE");
            await context.DisposeAsync();
        }
        #endregion
    }
}
