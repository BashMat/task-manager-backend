#region Usings

using System.Security.Claims;
using TaskManagerBackend.Application.Exceptions;

#endregion

namespace TaskManagerBackend.Application.Controllers;

/// <summary>
///     Represents abstract base Controller with common functionality for other Controllers.
/// </summary>
public abstract class ControllerBase : Microsoft.AspNetCore.Mvc.ControllerBase
{
    private const string InvalidTokenExceptionMessage = "Invalid token: user id was not provided or is invalid";
    
    /// <summary>
    ///     Returns User id from token provided with HTTP request. If token is not available, exception is thrown. 
    /// </summary>
    /// <exception cref="AccessDeniedException">
    ///     Exception is thrown if claim was not found or its content is not a valid int value.
    /// </exception>
    protected int UserId
    {
        get
        {
            string? claimValue = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (claimValue is null)
            {
                throw new AccessDeniedException(InvalidTokenExceptionMessage);
            }

            try
            {
                return Convert.ToInt32(claimValue);
            }
            catch (Exception e)
            {
                throw new AccessDeniedException(InvalidTokenExceptionMessage);
            }
        }
    }
}