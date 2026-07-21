using Application.Components;
using Application.Services.BookingService;
using Application.Services.EventService;
using Application.Services.PersonService;
using Application.Services.ReservationService;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class ApplicationInjection
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IReservationService, ReservationService>();
            services.AddHostedService<BookingProcessor>();
            services.AddSingleton<IPasswordHasher, SHA256PasswordHasher>();
        }
    }
}
