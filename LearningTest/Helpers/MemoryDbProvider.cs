using LearningWebApi.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LearningTest.Helpers
{
    internal static class MemoryDbProvider
    {
        public static IServiceCollection AppDbInMemoryConfigure(this IServiceCollection services)
        {
            var dbName = Guid.NewGuid().ToString();
            return services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(dbName));
        }
    }
}
