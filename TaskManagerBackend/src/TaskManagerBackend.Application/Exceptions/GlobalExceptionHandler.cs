#region Usings

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.Application.Utility;

#endregion

namespace TaskManagerBackend.Application.Exceptions;

/// <summary>
///     Represents main exception handler in ASP. NET Core application
///     used by the exception handler middleware.
/// </summary>
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,
                                                Exception exception,
                                                CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails();
        
        if (exception is IApplicationException ex)
        {
            logger.LogError(exception, "Application exception logged in global exception handler:");
            problemDetails.Title = ex.ResponseMessage;
            problemDetails.Status = ex.ActionResult.ToStatusCodesOrNull() ?? StatusCodes.Status500InternalServerError;
        }
        else
        {
            logger.LogError(exception, "Unknown system exception logged in global exception handler:");
            problemDetails.Title = "Internal Server Error";
            problemDetails.Status = StatusCodes.Status500InternalServerError;
            problemDetails.Detail = "Something went wrong. Try again later or contact service provider.";
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response
                         .WriteAsJsonAsync(problemDetails, cancellationToken);
        
        return true;
    }
}