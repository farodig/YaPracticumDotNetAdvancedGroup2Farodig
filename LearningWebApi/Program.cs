using LearningWebApi.Services.EventService;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSingleton<IEventService, EventService>();

var app = builder.Build();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();