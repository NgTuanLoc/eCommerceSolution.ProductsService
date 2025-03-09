using System.Net;

namespace ProductsMicroservice.API.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                // Log the exception type and message
                _logger.LogError(ex, "{ExceptionType}: {Message}", ex.GetType().ToString(), ex.Message);

                if (ex.InnerException is not null)
                {
                    // Log the inner exception type and message
                    _logger.LogError(ex.InnerException, "{ExceptionType}: {Message}", ex.InnerException.GetType().ToString(), ex.InnerException.Message);
                }

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                await httpContext.Response.WriteAsJsonAsync(new
                {
                    ex.Message,
                    Type = ex.GetType().ToString()
                });
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
