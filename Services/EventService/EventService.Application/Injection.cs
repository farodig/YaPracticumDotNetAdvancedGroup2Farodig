using EventService.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EventService.Application
{
    public static class Injection
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IEventService, EventService>();
        }
    }
}
