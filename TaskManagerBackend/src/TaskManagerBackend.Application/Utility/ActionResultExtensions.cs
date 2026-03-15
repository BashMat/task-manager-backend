using TaskManagerBackend.Domain;

namespace TaskManagerBackend.Application.Utility;

public static class ActionResultExtensions
{
    public static int? ToStatusCodesOrNull(this ActionResults actionResult)
    {
        return actionResult switch
               {
                   ActionResults.Success => StatusCodes.Status200OK,
                   ActionResults.UserError => StatusCodes.Status400BadRequest,
                   ActionResults.Unauthorized => StatusCodes.Status401Unauthorized,
                   ActionResults.AccessDenied => StatusCodes.Status403Forbidden,
                   ActionResults.ResourceNotFound => StatusCodes.Status404NotFound,
                   ActionResults.DataConflict => StatusCodes.Status409Conflict,
                   ActionResults.NotImplemented => StatusCodes.Status501NotImplemented,
                   ActionResults.ServerError => StatusCodes.Status500InternalServerError,
                   _ => null
               };
    }
}