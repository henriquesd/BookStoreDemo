using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

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

            var (statusCode, message, detail) = exception switch
            {
                DbUpdateConcurrencyException => (
                    StatusCodes.Status409Conflict,
                    "Concurrency conflict",
                    "The record was modified by another user. Please refresh and try again."
                ),
                DbUpdateException dbEx => (
                    StatusCodes.Status400BadRequest,
                    "Database operation failed",
                    _environment.IsDevelopment() ? dbEx.InnerException?.Message ?? dbEx.Message : "A database error occurred"
                ),
                ArgumentNullException nullEx => (
                    StatusCodes.Status400BadRequest,
                    "Missing required parameter",
                    nullEx.ParamName != null ? $"{nullEx.ParamName} is required" : "A required parameter is missing"
                ),
                ArgumentException argEx => (
                    StatusCodes.Status400BadRequest,
                    "Invalid argument",
                    argEx.Message
                ),
                InvalidOperationException => (
                    StatusCodes.Status400BadRequest,
                    "Invalid operation",
                    _environment.IsDevelopment() ? exception.Message : "The operation could not be completed"
                ),
                UnauthorizedAccessException => (
                    StatusCodes.Status401Unauthorized,
                    "Unauthorized",
                    "You are not authorized to perform this action"
                ),
                _ => (
                    StatusCodes.Status500InternalServerError,
                    "Internal server error",
                    _environment.IsDevelopment() ? exception.Message : "An unexpected error occurred"
                )
            };

            context.Response.StatusCode = statusCode;

            var response = new
            {
                message = message,
                detail = detail,
                type = exception.GetType().Name,
                traceId = context.TraceIdentifier,
                stackTrace = _environment.IsDevelopment() ? exception.StackTrace : null
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
