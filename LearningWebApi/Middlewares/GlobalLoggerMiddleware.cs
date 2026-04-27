namespace LearningWebApi.Middlewares
{
    /// <summary>
    /// Глобальное логирование
    /// </summary>
    internal class GlobalLoggerMiddleware(RequestDelegate next, ILogger<GlobalLoggerMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<GlobalLoggerMiddleware> _logger = logger;
        public async Task Invoke(HttpContext context)
        {
            var originalResponseBody = context.Response.Body;
            using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            await _next(context);

            await LogExpectedErrors(context, memoryStream).ConfigureAwait(false);

            memoryStream.Position = 0;
            await memoryStream.CopyToAsync(originalResponseBody);
        }

        private async Task LogExpectedErrors(HttpContext context, MemoryStream memoryStream)
        {
            if (context.Response.StatusCode == StatusCodes.Status307TemporaryRedirect)
            {
                return;
            }

            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                return;
            }

            switch (context.Response.StatusCode)
            {
                case StatusCodes.Status200OK:
                case StatusCodes.Status201Created:
                    {
                        _logger.LogInformation($"{context.Request.Method} {context.Request.Path} => {(System.Net.HttpStatusCode)context.Response.StatusCode}");
                        break;
                    }
                case StatusCodes.Status204NoContent:
                    {
                        _logger.LogInformation($"{context.Request.Method} {context.Request.Path} => {(System.Net.HttpStatusCode)context.Response.StatusCode}");
                        return;
                    }
                case StatusCodes.Status400BadRequest:
                case StatusCodes.Status404NotFound:
                case StatusCodes.Status500InternalServerError:
                default:
                    {
                        _logger.LogError($"{context.Request.Method} {context.Request.Path} => {(System.Net.HttpStatusCode)context.Response.StatusCode}");
                        break;
                    }
            }

            memoryStream.Position = 0;
            var responseBody = await new StreamReader(memoryStream).ReadToEndAsync().ConfigureAwait(false);
            _logger.LogInformation(responseBody);
        }
    }
}
