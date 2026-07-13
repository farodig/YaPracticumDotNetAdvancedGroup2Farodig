using Application.Services.BookingService;
using Application.Services.EventService;
using LearningWebApi.Services.BookingService;
using NLog.Web;
using System.Reflection;

namespace LearningWebApi
{
    /// <summary>
    /// Расширения для добавления сервисов
    /// </summary>
    internal static class ServiceProviderExtension
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

        /// <summary>
        /// Добавить swagger
        /// </summary>
        public static void AddSwaggerService(this WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(options =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath, true);
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSwagger",
                    policy =>
                    {
                        // Читаем из настроек или используем значения по умолчанию
                        var origins = (builder.Configuration["ASPNETCORE_URLS"] ??
                                   builder.Configuration["urls"] ?? string.Empty)
                                   .Split(';');

                        policy.WithOrigins(origins)
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
            });
        }

        /// <summary>
        /// Добавить Nlog
        /// </summary>
        public static void AddNlog(this WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();
        }
    }
}
