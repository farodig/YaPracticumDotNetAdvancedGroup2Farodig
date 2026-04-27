using LearningWebApi.Middlewares;
using LearningWebApi.Repositories;
using LearningWebApi.Services.EventService;
using NLog.Web;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSingleton<IEventRepository, EventRepository>();
builder.Services.AddSingleton<IEventService, EventService>();
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


//builder.Logging.ClearProviders();
//builder.Host.UseNLog();

var app = builder.Build();

//app.UseMiddleware<GlobalLoggerMiddleware>();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowSwagger");
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();