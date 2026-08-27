using Microsoft.OpenApi;
using System.Reflection;

namespace BookingService.Presentation.ConfigurationBuilders
{
    internal static class SwaggerBuilder
    {
        /// <summary>
        /// Добавить swagger
        /// </summary>
        public static void ConfigureSwaggerService(this WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(options =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath, true);

                options.AddSecurityDefinition("Bearer", securityScheme: new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter the Bearer token. Format: Bearer <token>",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });
                options.AddSecurityRequirement(document =>
                                new OpenApiSecurityRequirement
                                {
                                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                                });
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

        public static void InitializeSwagger(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseCors("AllowSwagger");
                app.UseSwagger();
                app.UseSwaggerUI();


                app.Lifetime.ApplicationStarted.Register(() =>
                {
                    var addresses = string.Join(", ", app.Urls.Select(u => $"{u}/swagger"));
                    var logger = app.Services.GetRequiredService<ILogger<Program>>();
                    if (logger.IsEnabled(LogLevel.Information))
                    {
                        logger.LogInformation("Booking Swagger UI: {Addresses}", addresses);
                    }
                });
            }
        }
    }
}
