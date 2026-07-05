using LearningWebApi;
using LearningWebApi.DataAccess;
using LearningWebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.AppDbConfigure();
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
app.AppDbInitialize();

app.Run();