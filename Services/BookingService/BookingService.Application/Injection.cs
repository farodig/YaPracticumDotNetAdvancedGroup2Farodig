using BookingService.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace BookingService.Application
{
    public static class Injection
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IBookingService, BookingService>();
            services.AddHostedService<BookingProcessor>();
        }
    }
}
