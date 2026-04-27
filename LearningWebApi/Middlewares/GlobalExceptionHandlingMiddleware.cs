using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace LearningWebApi.Middlewares
{
    internal class GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger = logger;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);

                var problemDetails = CreateProblemDetails(context);

                context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/problem+json";

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                await context.Response.WriteAsJsonAsync(problemDetails, options);
            }
        }

        private ProblemDetails CreateProblemDetails(HttpContext context)
        {
            // Все отловленные ошибки здесь являются внутренними ошибками сервера
            // дополнительную информацию всегда можно посмотреть в логах
            var problemDetails = new ProblemDetails()
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = "Check log file for detail"
            };

            problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier;
            return problemDetails;
        }
    }
}