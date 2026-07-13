using System.Reflection;

namespace LearningWebApi.ConfigurationBuilders
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
            }
        }
    }
}
