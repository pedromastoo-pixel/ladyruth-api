using System.Net;
using System.Text.Json;

namespace LadyRuth.API.Middleware;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await WriteErrorResponse(context, ex);
        }
    }

    private static async Task WriteErrorResponse(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode  = ex switch
        {
            InvalidOperationException  => (int)HttpStatusCode.BadRequest,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            _                          => (int)HttpStatusCode.InternalServerError
        };

        var response = new
        {
            message = context.Response.StatusCode == 500
                ? "An unexpected error occurred. Please try again."
                : ex.Message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
