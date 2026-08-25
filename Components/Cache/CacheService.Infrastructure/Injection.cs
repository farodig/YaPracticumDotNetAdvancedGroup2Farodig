using CacheService.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace CacheService.Infrastructure
{
    public static class Injection
    {
        public static void AddCacheServiceFactory(this WebApplicationBuilder builder)
        {
            var section = builder.Configuration.GetSection("Cache");
            builder.Services.ConfigureCache<RedisCacheSettings>(section);
            builder.Services.AddRedis();
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

        private static void AddRedis(this IServiceCollection services)
        {
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var optionsSnapshot = sp.GetRequiredService<IOptions<RedisCacheSettings>>().Value;
                var options = ConfigurationOptions.Parse(optionsSnapshot.ConnectionString);
                options.AbortOnConnectFail = false;
                return ConnectionMultiplexer.Connect(options);
            });
        }
    }
}