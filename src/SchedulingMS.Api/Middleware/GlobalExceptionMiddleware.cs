using SchedulingMS.Application.Exceptions;
using SchedulingMS.Api.Common;

namespace SchedulingMS.Api.Middleware;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception for request {Method} {Path}", context.Request.Method, context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var (statusCode, code) = ex switch
        {
            ValidationException => (StatusCodes.Status400BadRequest, "validation_error"),
            ConflictException => (StatusCodes.Status409Conflict, "domain_conflict"),
            NotFoundException => (StatusCodes.Status404NotFound, "not_found"),
            _ => (StatusCodes.Status500InternalServerError, "internal_server_error")
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        var body = new ApiError(
            code,
            ex.Message,
            statusCode,
            context.TraceIdentifier,
            DateTime.UtcNow);

        return context.Response.WriteAsJsonAsync(body);
    }
}
