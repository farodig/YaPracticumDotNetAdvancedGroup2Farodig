using EventService.Presentation.ConfigurationBuilders;
using EventService.Presentation.Middlewares;
using TokenService;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.ConfigureInfrastructure();
builder.ConfigureApplication();
builder.ConfigureSwaggerService();
builder.ConfigureAuthentication();
builder.ConfigureLog();
builder.ConfigureTelemetry();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.InitializeSwagger();
app.MapControllers();
app.InitializeInfrastructure();
app.MapPrometheusScrapingEndpoint();

app.Run();