using Microsoft.AspNetCore.Mvc;
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

                var problemDetails = CreateProblemDetails(context, ex);

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

        private ProblemDetails CreateProblemDetails(HttpContext context, Exception ex)
        {
            var problemDetails = new ProblemDetails
            {
                Instance = context.Request.Path,
            };

            switch (ex)
            {
                case ArgumentException argEx:
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Title = "Invalid Argument";
                    problemDetails.Detail = argEx.Message;
                    break;

                case KeyNotFoundException:
                    problemDetails.Status = StatusCodes.Status404NotFound;
                    problemDetails.Title = "Resource Not Found";
                    problemDetails.Detail = ex.Message;
                    break;

                case InvalidOperationException invalidOpEx:
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Title = "Invalid Operation";
                    problemDetails.Detail = invalidOpEx.Message;
                    break;

                default:
                    problemDetails.Status = StatusCodes.Status500InternalServerError;
                    problemDetails.Title = "Internal Server Error";
                    problemDetails.Detail = "An unexpected error occurred while processing your request";
                    break;
            }

            return problemDetails;
        }
    }
}