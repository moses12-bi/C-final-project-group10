using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace ProjectM.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt");
                await HandleExceptionAsync(context, HttpStatusCode.Unauthorized, "Unauthorized access");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument");
                await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found");
                await HandleExceptionAsync(context, HttpStatusCode.NotFound, "Resource not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, "An error occurred");
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                error = new
                {
                    message = message,
                    statusCode = (int)statusCode
                }
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
