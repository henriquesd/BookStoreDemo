using System.Net;
using System.Text.Json;

namespace BookStore.API.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _environment;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(
            RequestDelegate next,
            IHostEnvironment environment,
            ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _environment = environment;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred while processing {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = _environment.IsDevelopment()
                ? new
                {
                    message = "An unexpected error occurred",
                    detail = exception.Message,
                    stackTrace = exception.StackTrace
                }
                : new
                {
                    message = "An unexpected error occurred",
                    detail = (string?)null,
                    stackTrace = (string?)null
                };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }

    public static class GlobalExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }
}
