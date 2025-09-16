using Microsoft.AspNetCore.Mvc;
using SubManager.Application.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace SubManager.API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _appEnv;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IWebHostEnvironment appEnv)
        {
            _next = next;
            _logger = logger;
            _appEnv = appEnv;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception e)
            {
                httpContext.Response.ContentType = "application/json";
                int statusCode;
                string errorMessage;

                // Check to refacto a bit
                switch (e)
                {
                    case NotFoundException:
                        statusCode = StatusCodes.Status404NotFound;
                        errorMessage = e.Message;
                        break;
                    case ValidationException:
                        statusCode = StatusCodes.Status400BadRequest;
                        errorMessage = e.Message;
                        break;
                    case UnauthorizedAccessException:
                        statusCode = StatusCodes.Status401Unauthorized;
                        errorMessage = e.Message;
                        break;
                    case InvalidOperationException:
                        statusCode = StatusCodes.Status500InternalServerError;
                        errorMessage = "A configuration error occurred: " + e.Message;
                        break;
                    default:
                        statusCode = StatusCodes.Status500InternalServerError;
                        errorMessage = "An unexpected error occurred.";
                        break;
                }

                _logger.LogError(e, "An exception occurred: {Message}", e.Message);

                var problemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = errorMessage,
                    Detail = _appEnv.IsDevelopment() ? e.StackTrace : null,
                    Instance = httpContext.Request.Path
                };

                httpContext.Response.StatusCode = statusCode;
                await httpContext.Response.WriteAsJsonAsync(problemDetails);
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
