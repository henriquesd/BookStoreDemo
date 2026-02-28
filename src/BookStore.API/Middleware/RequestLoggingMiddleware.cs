using System.Diagnostics;

namespace BookStore.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = GetOrCreateCorrelationId(context);

            // Add correlation ID to response headers
            context.Response.Headers.Add("X-Correlation-ID", correlationId);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                // Log request
                _logger.LogInformation(
                    "HTTP {Method} {Path} started. CorrelationId: {CorrelationId}, RemoteIp: {RemoteIp}",
                    context.Request.Method,
                    context.Request.Path,
                    correlationId,
                    context.Connection.RemoteIpAddress?.ToString() ?? "Unknown");

                // Create a scope with correlation ID for all logs in this request
                using (_logger.BeginScope(new Dictionary<string, object>
                {
                    ["CorrelationId"] = correlationId,
                    ["RequestPath"] = context.Request.Path.ToString(),
                    ["RequestMethod"] = context.Request.Method
                }))
                {
                    await _next(context);
                }

                stopwatch.Stop();

                // Log response
                _logger.LogInformation(
                    "HTTP {Method} {Path} completed with {StatusCode} in {ElapsedMilliseconds}ms. CorrelationId: {CorrelationId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds,
                    correlationId);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(ex,
                    "HTTP {Method} {Path} failed with exception after {ElapsedMilliseconds}ms. CorrelationId: {CorrelationId}",
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds,
                    correlationId);

                throw; // Re-throw to let the global exception handler deal with it
            }
        }

        private static string GetOrCreateCorrelationId(HttpContext context)
        {
            // Check if client sent a correlation ID
            if (context.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId)
                && !string.IsNullOrWhiteSpace(correlationId))
            {
                return correlationId.ToString();
            }

            // Generate a new correlation ID
            return Guid.NewGuid().ToString();
        }
    }

    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
