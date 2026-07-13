using Application.Services.BookingService;
using Application.Services.EventService;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class ApplicationInjection
    {
        /// <summary>
        /// Добавить сервис событий
        /// </summary>
        public static void AddEventService(this IServiceCollection services)
        {
            services.AddScoped<IEventService, EventService>();
        }

        /// <summary>
        /// Добавить сервис бронирований
        /// </summary>
        /// <param name="services"></param>
        public static void AddBookingService(this IServiceCollection services)
        {
            services.AddScoped<IBookingService, BookingService>();
            services.AddHostedService<BookingProcessor>();
        }
    }
}
