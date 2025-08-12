using System.Net;

using Serilog;

namespace Presentation.Middlewares;

public class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
    }

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            return _next(context);
        }
        catch (Exception ex)
        {
            Log.Error($"Unhandled error: {ex.Message}");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new
            {
                statusCode = context.Response.StatusCode,
                message = "An unexpected error occurred. Please try again later."
            };

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}