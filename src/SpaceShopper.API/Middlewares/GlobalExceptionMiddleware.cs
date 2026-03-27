using Microsoft.AspNetCore.Http;
using SpaceShopper.API.Models;
using SpaceShopper.Application.Common.Exceptions;
using SpaceShopper.Application.Common.Errors;

namespace SpaceShopper.API.Middlewares
{
    public sealed class GlobalExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning(ex, "Business exception occurred");

                var statusCode = ex switch
                {
                    ValidationException => StatusCodes.Status400BadRequest,
                    UnauthorizedException => StatusCodes.Status401Unauthorized,
                    NotFoundException => StatusCodes.Status404NotFound,
                    DomainException => StatusCodes.Status409Conflict,
                    _ => StatusCodes.Status400BadRequest
                };

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                var response = ApiResponse<object>.Fail(ex.ErrorCode, ex.Message, ex.Details);
                await context.Response.WriteAsJsonAsync(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var response = ApiResponse<object>.Fail(ErrorCodes.Infrastructure.Unknown, "Internal server error");
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
