using CacheService.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CacheService.Infrastructure
{
    public static class Injection
    {
        public static void AddReceiveFactory(this WebApplicationBuilder builder)
        {
            var section = builder.Configuration.GetSection("Cache");
            builder.Services.ConfigureCache<RedisCacheSettings>(section);
            builder.Services.AddSingleton<ICacheServiceFactory, RedisCacheServiceFactory>();
        }

        public static void ConfigureCache<TOptions>(this IServiceCollection services, IConfigurationSection section)
            where TOptions : class
        {
            services.Configure<TOptions>(section);
            services.AddOptions<TOptions>()
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }
    }
}