using Presentation.Middlewares;
using Presentation.ConfigurationBuilders;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.ConfigureInfrastructure();
builder.ConfigureApplication();
builder.ConfigureSwaggerService();
builder.ConfigureAuthentication();
builder.AddNlog();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.InitializeSwagger();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.InitializeInfrastructure();

app.Run();