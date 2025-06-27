#region Usings

using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.Application.Exceptions;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Domain;

#endregion

namespace TaskManagerBackend.Application.Features;

/// <summary>
///     Represents abstract base Controller with common functionality for other Controllers.
/// </summary>
public abstract class ControllerBase : Microsoft.AspNetCore.Mvc.ControllerBase
{
    private const string InvalidTokenExceptionMessage = "Invalid token: user id was not provided or is invalid. Possibly it was requested in method that does not require token, which may mean error in endpoint configuration.";

    /// <summary>
    ///     Returns User id from token provided with HTTP request. If token is not available, exception is thrown. 
    /// </summary>
    /// <exception cref="InvalidTokenException">
    ///     Exception is thrown if claim was not found or its content is not a valid int value.
    /// </exception>
    /// <remarks>
    ///     Currently User id is expected as required claim in token. However, this field is available even
    ///     in Controllers with endpoints not protected with Authorize attribute, which leads to possible
    ///     <see cref="NullReferenceException"/>. Explicit exception was created for this situation.
    /// </remarks>
    protected int UserId
    {
        get
        {
            string? claimValue = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (claimValue is null)
            {
                throw new InvalidTokenException(InvalidTokenExceptionMessage);
            }

            try
            {
                return Convert.ToInt32(claimValue);
            }
            catch
            {
                throw new InvalidTokenException(InvalidTokenExceptionMessage);
            }
        }
    }

    // TODO: Try to move conversion into ServiceResponse<T> as user-defined implicit conversion.
    protected ObjectResult ConvertServiceResponse<T>(ServiceResponse<T> response)
    {
        switch (response.ActionResult)
        {
            case ActionResults.Success:
                return Ok(response);
            case ActionResults.UserError:
            case ActionResults.Unauthorized:
            case ActionResults.AccessDenied:
            case ActionResults.ResourceNotFound:
            case ActionResults.DataConflict:
            case ActionResults.ServerError:
            default:
                return Problem(detail: response.Message,
                               statusCode: response.StatusCode);
        }
    }
}