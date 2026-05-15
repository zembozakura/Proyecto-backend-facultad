using System.Net;
using System.Text.Json;

namespace MiApp.WebApi.Middleware;

/// <summary>
/// Middleware global para manejar excepciones no capturadas
/// Devuelve respuestas JSON consistentes
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "Excepción no manejada");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        return exception switch
        {
            KeyNotFoundException => 
                HandleKeyNotFoundException(context, exception),
            
            InvalidOperationException => 
                HandleInvalidOperationException(context, exception),
            
            _ => HandleGenericException(context, exception)
        };
    }

    private static Task HandleKeyNotFoundException(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        return context.Response.WriteAsJsonAsync(new
        {
            success = false,
            message = exception.Message
        });
    }

    private static Task HandleInvalidOperationException(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        return context.Response.WriteAsJsonAsync(new
        {
            success = false,
            message = exception.Message
        });
    }

    private static Task HandleGenericException(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        return context.Response.WriteAsJsonAsync(new
        {
            success = false,
            message = "Ocurrió un error interno",
            details = exception.Message
        });
    }
}

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}
