using EventService.Application.Abstractions;
using EventService.Application.EventProcessors;
using Microsoft.Extensions.DependencyInjection;

namespace EventService.Application
{
    public static class Injection
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IEventService, EventService>();
            services.AddHostedService<BookingSuccessEventProcessor>();
            services.AddHostedService<BookingCancelEventProcessor>();
        }
    }
}
