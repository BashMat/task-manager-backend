using TaskManagerBackend.Domain.Shared.Workflow;

namespace TaskManagerBackend.Application.Utility;

public static class ActionResultExtensions
{
    public static int? ToStatusCodesOrNull(this ActionResultType actionResultType)
    {
        if (ActionResultTypeToStatusCode.TryGetValue(actionResultType, out int statusCode))
        {
            return statusCode;
        }

        return null;
    }
    
    private static Dictionary<ActionResultType, int> ActionResultTypeToStatusCode { get; } = new()
        {
            {
                ActionResultType.Success,
                StatusCodes.Status200OK
            },
            {
                ActionResultType.UserError,
                StatusCodes.Status400BadRequest
            },
            {
                ActionResultType.Unauthenticated,
                StatusCodes.Status401Unauthorized
            },
            {
                ActionResultType.Unauthorized,
                StatusCodes.Status403Forbidden
            },
            {
                ActionResultType.ResourceNotFound,
                StatusCodes.Status404NotFound
            },
            {
                ActionResultType.DataConflict,
                StatusCodes.Status409Conflict
            },
            {
                ActionResultType.ServerError,
                StatusCodes.Status500InternalServerError
            },
            {
                ActionResultType.NotImplemented,
                StatusCodes.Status501NotImplemented
            }
        };
}