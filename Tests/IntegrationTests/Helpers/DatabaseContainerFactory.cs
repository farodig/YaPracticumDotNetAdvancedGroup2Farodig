using DotNet.Testcontainers.Containers;
using Testcontainers.PostgreSql;

namespace Learning.IntegrationTests.Helpers
{
    internal static class DatabaseContainerFactory
    {
        public static IDatabaseContainer CreatePostgreSqlContainer() => new PostgreSqlBuilder("postgres:16-alpine")
            .WithName("test-postgres-reuse")
            .WithReuse(true)
            .Build();

        public static IDatabaseContainer CreateTestPostgeSqlContaner() => new TestPostgeSqlContaner("Host=localhost;Port=5432;Database=testeventapi;Username=postgres;Password=postgres");
    }
}
