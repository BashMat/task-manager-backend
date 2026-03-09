using Microsoft.AspNetCore.Diagnostics;

namespace TaskManagerBackend.Application.Exceptions;

/// <summary>
///     Represents main exception handler in ASP. NET Core application
///     used by the exception handler middleware.
/// </summary>
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(HttpContext httpContext,
                                          Exception exception,
                                          CancellationToken cancellationToken)
    {
        if (exception is IApplicationException ex)
        {
            logger.LogError(exception, "Application exception logged in global exception handler:");
            httpContext.Response.StatusCode = MapExceptionToStatusCode(ex);
        }
        else
        {
            logger.LogError(exception, "Unrecognised system exception logged in global exception handler:");
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return new ValueTask<bool>(true);
    }

    private int MapExceptionToStatusCode(IApplicationException applicationException)
    {
        // TODO: Implement better mapping
        // Interface should describe internal domain error code,
        // and it should be mapped to status code. Currently exception itself is mapped.

        switch (applicationException)
        {
            case InvalidTokenException:
                return StatusCodes.Status401Unauthorized;
            default:
                logger.LogError("Unrecognised application error");
                return StatusCodes.Status500InternalServerError;
        }
    }
}