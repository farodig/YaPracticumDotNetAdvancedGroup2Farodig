using Presentation.Middlewares;
using Presentation.ConfigurationBuilders;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.ConfigureInfrastructure();
builder.ConfigureApplication();
builder.ConfigureSwaggerService();

builder.AddNlog();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.InitializeSwagger();
app.UseHttpsRedirection();
app.MapControllers();
app.InitializeInfrastructure();

app.Run();