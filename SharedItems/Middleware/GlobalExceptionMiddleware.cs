using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SharedItems.Exceptions;
using System.Net;
using System.Text.Json;

namespace SharedItems.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            int statusCode;
            string message = "An unexpected error occurred.";
            string details = string.Empty;

            switch (exception)
            {
                case UnauthorizeException unauth:
                    statusCode = StatusCodes.Status401Unauthorized;
                    message = unauth.Message;
                    details = unauth.Details;
                    break;;
                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    message = exception.Message;
                    details = exception.StackTrace ?? string.Empty;
                    break;
            }

            _logger.LogError(exception, "Unhandled exception occurred while processing request");

            var result = JsonSerializer.Serialize(new { statusCode, message, details }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(result);
        }
    }

    public static class GlobalExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}
