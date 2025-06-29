#region Usings

using System.Text.Json.Serialization;
using TaskManagerBackend.Domain;

#endregion

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
    
    public T? Data { get; init; }
    
    [JsonIgnore]
    public ActionResults ActionResult { get; init; }
    public bool Success => Data != null && ActionResult == ActionResults.Success;
    public string? Message { get; init; }
    
    [JsonIgnore]
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