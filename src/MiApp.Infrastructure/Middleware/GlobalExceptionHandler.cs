using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MiApp.Domain.Exceptions;

namespace MiApp.Infrastructure.Middleware;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) =>
        _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        var (statusCode, title) = exception switch
        {
            NotFoundException      => (StatusCodes.Status404NotFound,     exception.Message),
            ValidationException    => (StatusCodes.Status400BadRequest,   "Validation failed"),
            DomainException        => (StatusCodes.Status422UnprocessableEntity, exception.Message),
            _                      => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status   = statusCode,
                Title    = title,
                Instance = httpContext.Request.Path
            },
            cancellationToken);

        return true;
    }
}
