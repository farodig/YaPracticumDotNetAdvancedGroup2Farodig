
using Application;
using LearningWebApi;
using LearningWebApi.Middlewares;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.ConfigureInfrastructure();
builder.Services.AddRepositories();
builder.Services.AddEventService();
builder.Services.AddBookingService();
builder.AddSwaggerService();

builder.AddNlog();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowSwagger");
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Services.InitializeInfrastructure();

app.Run();