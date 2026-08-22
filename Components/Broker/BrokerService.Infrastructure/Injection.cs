using BrokerService.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BrokerService.Infrastructure
{
    public static class Injection
    {
        public static void AddPublishService(this WebApplicationBuilder builder)
        {
            var section = builder.Configuration.GetSection("Broker");
            builder.Services.ConfigureBroker<KafkaSettings>(section);
            builder.Services.AddSingleton<IPublishService, KafkaPublisher>();
        }

        public static void AddReceiveService(this WebApplicationBuilder builder)
        {
            var section = builder.Configuration.GetSection("Broker");
            builder.Services.ConfigureBroker<KafkaSettings>(section);
            builder.Services.AddScoped<IReceiveService, KafkaReceiver>();
        }

        public static void ConfigureBroker<TOptions>(this IServiceCollection services, IConfigurationSection section)
            where TOptions : class
        {
            services.Configure<TOptions>(section);
            services.AddOptions<TOptions>()
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }
    }
}
