using Application.Components;
using Application.Services.BookingService;
using Application.Services.EventService;
using Application.Services.ReservationService;
using Application.Services.TokenService;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class ApplicationInjection
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IReservationService, ReservationService>();
            services.AddHostedService<BookingProcessor>();
            services.AddSingleton<ITokenService, TokenService>();
            services.AddSingleton<IPasswordHasher, SHA256PasswordHasher>();
        }
    }
}
