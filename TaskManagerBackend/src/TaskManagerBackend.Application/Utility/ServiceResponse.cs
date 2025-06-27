using TaskManagerBackend.Domain;

namespace TaskManagerBackend.Application.Utility;

public class ServiceResponse<T>
{
    public ServiceResponse(T? data = default,
                           ActionResults actionResult = ActionResults.Success,
                           string? message = null)
    {
        Data = data;
        ActionResult = actionResult;
        Message = message;
    }
    
    public static implicit operator ServiceResponse<T>(T? data)
    {
        return new ServiceResponse<T>(data);
    }
    
    public T? Data { get; set; }
    public ActionResults ActionResult { get; set; }
    public bool Success => ActionResult == ActionResults.Success;
    public string? Message { get; set; }
    public int? StatusCode => ConvertActionResultToStatusCode();
    
    private int? ConvertActionResultToStatusCode()
    {
        return ActionResult switch
               {
                   ActionResults.Success => StatusCodes.Status200OK,
                   ActionResults.UserError => StatusCodes.Status400BadRequest,
                   ActionResults.Unauthorized => StatusCodes.Status401Unauthorized,
                   ActionResults.AccessDenied => StatusCodes.Status403Forbidden,
                   ActionResults.ResourceNotFound => StatusCodes.Status404NotFound,
                   ActionResults.DataConflict => StatusCodes.Status409Conflict,
                   ActionResults.ServerError => StatusCodes.Status500InternalServerError,
                   _ => null
               };
    }
}